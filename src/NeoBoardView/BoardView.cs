using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Core.Math;

namespace NeoBoardView
{
    internal interface IPart
    {
        string Name { get; set; }
        BoardSide Side { get; set; }
        int FirstPin { get; set; }
        int PinCount { get; set; }
    }

    internal interface IPin
    {
        Vector2 Location { get; set; }
        int Net { get; set; }
        string Name { get; set; }
        BoardSide Side { get; set; }
        IPin Inverse { get; set; }
    }

    internal interface INail
    {
        Vector2 Location { get; set; }
        int Net { get; set; }
        BoardSide Side { get; set; }
        string Name { get; set; }
    }

    internal class Part : IPart
    {
        public string Name { get; set; }
        public BoardSide Side { get; set; }
        public int FirstPin { get; set; }
        public int PinCount { get; set; }
    }

    internal class Pin : IPin
    {
        public Vector2 Location { get; set; }
        public int Net { get; set; }
        //public int PartIndex; // XXX: remove
        public string Name { get; set; }
        public BoardSide Side { get; set; }
        public IPin Inverse { get; set; }
    }

    internal class Nail : INail
    {
        public int Id { get; set; }
        public byte Type { get; set; }
        public Vector2 Location { get; set; }
        public int Net { get; set; }
        public BoardSide Side { get; set; }
        public string Name { get; set; }
    }

    internal interface IBoardViewFile
    {
        bool FlipBottomY { get; }
        bool InchUnits { get; }// = false; // 'INCH' or ?
        Vector2 OriginLocation { get; } //= Vector2.Origin;
        Angle OriginTurn { get; } //= 0;
        List<Vector2> TopContour { get; }
        List<Vector2> BottomContour { get; }
        List<IPart> Parts { get; }
        List<IPin> Pins { get; }
        List<INail> Nails { get; }
        bool Load(string path);
    }

    [Flags]
    internal enum BoardSide
    {
        Top = 1,
        Bottom = 2,
        Both = Top|Bottom
    }

    internal enum FileFormat
    {
        TestLink, // BRD
        Honhan, // BDV
        Toptest, // plaintext BRD
        Unknown = -1
    }

    internal static class BoardUtil
    {
        public static BoardSide GetSide(int code)
        {
            switch (code)
            {
            case 0:
                return BoardSide.Both;
            case 1:
            case 5:
                return BoardSide.Top;
            case 2:
            case 10:
                return BoardSide.Bottom;
            default:
                return BoardSide.Top;
            }
        }

        public static string[] SplitLine(StringBuilder sb, int count)
        {
            var result = new string[count];
            int first = 0;
            int done = 0;
            while (done<count)
            {
                // skip whitespace
                while (first<sb.Length && sb[first]==' ')
                    first++;
                int next = first+1;
                while (next<sb.Length && sb[next]!=' ')
                    next++;
                result[done++] = first>=sb.Length ? null : sb.ToString(first, next-first);
                first = next;
            }
            return result;
        }

        public static string[] SplitLine(string sb)
        {
            char[] sep = {' '};
            return sb.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        }

        public static FileFormat DetectFormat(string path)
        {
            if (TestLinkBrdFile.Detect(path))
                return FileFormat.TestLink;
            if (ToptestBrdFile.Detect(path))
                return FileFormat.Toptest;
            if (HonhanBdvFile.Detect(path))
                return FileFormat.Honhan;
            return FileFormat.Unknown;
        }

        public static void OnUnknownFormat(string fname, int lineIndex)
        {
            var msg = string.Format("Can't load {0}:\r\n Unsupported format or corrupted file.\r\n[{1}]",
                fname, lineIndex);
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
    }
    
    internal class TestLinkBrdFile : IBoardViewFile
    {
        public static bool Detect(string path)
        {
            try
            {
                var sb = new StringBuilder(1024);
                using (var r = new StreamReader(path, Encoding.GetEncoding("iso-8859-1")))
                {
                    var str = r.ReadLine();
                    Decode(str, sb);
                    if (sb.ToString()=="str_length:")
                        return true;
                }
            }
            catch
            {}
            return false;
        }

        private static void Decode(string src, StringBuilder dst)
        {
            if (src.Length==0)
            {
                dst[0] = '\0';
                dst.Length = 0;
                return;
            }
            dst.Length = dst.Capacity;
            int i;
            for (i = 0; ; i++)
            {
                if (i==src.Length)
                    break;
                byte srci = (byte)(src[i]);
                char n = (char)(byte)(~(4*srci | ((srci & 0xC0) >> 6)));
                if (n==(char)0xD7)
                    n = '\n';
                dst[i] = n;
            }
            dst[i] = '\0';
            dst.Length = i;
        }

        private enum State
        {
            Idle,
            StrLength,
            VarData,
            Format,
            Parts,
            Pins,
            Nails,
        }
        public bool FlipBottomY { get { return true; } }
        public bool InchUnits { get; private set; }
        public Vector2 OriginLocation { get; private set; }
        public Angle OriginTurn { get; private set; }
        public List<Vector2> TopContour { get; private set; }
        public List<Vector2> BottomContour { get; private set; }
        public List<IPart> Parts { get; private set; }
        private List<IPart> nailParts = new List<IPart>(1000);
        private int prevPartLastPin = -1;
        public List<IPin> Pins { get; private set; }
        public List<INail> Nails { get; private set; }

        public TestLinkBrdFile()
        {
            InchUnits = false; // 'INCH' or ?
            OriginLocation = Vector2.Origin;
            OriginTurn = 0;
            TopContour = new List<Vector2>(1024);
            BottomContour = new List<Vector2>(1024);
            Parts = new List<IPart>(1024);
            Pins = new List<IPin>(1024);
            Nails = new List<INail>(1024);
        }

        private void Update(ref State currentState, StringBuilder data)
        {
            if (data.Length==0)
            {
                currentState = State.Idle;
                return;
            }
            switch (currentState)
            {
            case State.Idle:
                switch (data.ToString())
                {
                case "str_length:":
                    currentState = State.StrLength;
                    break;
                case "var_data:":
                    currentState = State.VarData;
                    break;
                case "Format:":
                    currentState = State.Format;
                    break;
                case "Parts:":
                    currentState = State.Parts;
                    break;
                case "Pins:":
                    currentState = State.Pins;
                    break;
                case "Nails:":
                    currentState = State.Nails;
                    break;
                default:
                    break; // leave in idle state
                }
                break;
            case State.StrLength: // XXX: wtf is this?
                break;
            case State.VarData: // XXX: wtf is this?
                break;
            case State.Format: // update contour
            {
                var str = BoardUtil.SplitLine(data, 2);
                var newVert = new Vector2(int.Parse(str[0]), int.Parse(str[1]));
                if (TopContour.Count>0 && TopContour[TopContour.Count-1]==newVert)
                    break;
                TopContour.Add(newVert);
                break;
            }
            case State.Parts: // update partsTest
            {
                var str = BoardUtil.SplitLine(data, 3);
                var part = new Part();
                part.Name = str[0];
                if (part.Name=="...")
                    part.Name = string.Empty;
                part.Side = BoardUtil.GetSide(int.Parse(str[1]));
                part.FirstPin = prevPartLastPin+1;
                part.PinCount = int.Parse(str[2])-part.FirstPin;
                prevPartLastPin = part.FirstPin+part.PinCount-1;
                if (part.Name.Length==0 && part.PinCount==1)
                {
                    nailParts.Add(part);
                    break;
                }
                Parts.Add(part);
                break;
            }
            case State.Pins: // update pins
            {
                var str = BoardUtil.SplitLine(data, 5);
                var pin = new Pin();
                pin.Location = new Vector2(int.Parse(str[0]), int.Parse(str[1]));
                pin.Net = int.Parse(str[2]);
                //pin.PartIndex = int.Parse(str[3]);
                pin.Name = str[4];
                Pins.Add(pin);
                break;
            }
            case State.Nails: // update nails
            {
                // net, x, y, side?, name
                var str = BoardUtil.SplitLine(data, 5);
                var nail = new Nail();
                nail.Location = new Vector2(int.Parse(str[1]), int.Parse(str[2]));
                nail.Net = int.Parse(str[0]);
                nail.Side = BoardUtil.GetSide(int.Parse(str[3]));
                nail.Name = str[4];
                if (nail.Name=="...")
                    nail.Name = string.Empty;
                Nails.Add(nail);
                break;
            }
            }
        }

        public bool Load(string path)
        {
            var sb = new StringBuilder(1024);
            using (var r = new StreamReader(path, Encoding.GetEncoding("iso-8859-1")))
            {
                int lineIndex = 1;
                var state = State.Idle;
                while (!r.EndOfStream)
                {
                    try
                    {
                        var str = r.ReadLine();
                        Decode(str, sb);
                        Update(ref state, sb);
                        lineIndex++;
                    }
                    catch (Exception e)
                    {
                        var fname = Path.GetFileName(path);
                        BoardUtil.OnUnknownFormat(fname, lineIndex);
                        return false;
                    }
                }
            }
            // process nail parts
            foreach (var part in nailParts)
            {
                var nail = new Nail();
                nail.Location = Pins[part.FirstPin].Location;
                nail.Net = Pins[part.FirstPin].Net;
                nail.Side = part.Side;
                nail.Name = Pins[part.FirstPin].Name;
                nail.Id = -99;
                Nails.Add(nail);
            }
            nailParts.Clear();
            // assign pin sides
            foreach (var part in Parts)
            {
                for (int i = part.FirstPin; i<part.FirstPin+part.PinCount; i++)
                    Pins[i].Side = part.Side;
            }
            // generate bottom contour
            for (int i = 0; i<TopContour.Count; i++)
            {
                var v = TopContour[i];
                v.Y *= -1;
                BottomContour.Add(v);
            }
            return true;
        }
    }

    internal class ToptestBrdFile : IBoardViewFile
    {
        public static bool Detect(string path)
        {
            try
            {
                using (var r = new StreamReader(path, Encoding.GetEncoding("iso-8859-1")))
                {
                    while (!r.EndOfStream)
                    {
                        r.ReadLine();
                        var str = r.ReadLine();
                        var spl = BoardUtil.SplitLine(str);
                        if (spl.Length==4 && spl[0].ToLowerInvariant()=="brdout:")
                            return true;
                    }
                }
            }
            catch
            {}
            return false;
        }

        private enum State
        {
            Idle,
            Format,
            Nets,
            Parts,
            Pins,
            Nails,
        }

        public bool FlipBottomY { get { return false; } }
        public bool InchUnits { get; private set; }
        public Vector2 OriginLocation { get; private set; }
        public Angle OriginTurn { get; private set; }
        public List<Vector2> TopContour { get; private set; }
        public List<Vector2> BottomContour { get; private set; }
        public List<IPart> Parts { get; private set; }
        public List<IPin> Pins { get; private set; }
        public List<INail> Nails { get; private set; }
        private string[] netNames;
        private Part prevPart = null;
        private int prevNet = 0;

        public ToptestBrdFile()
        {
            InchUnits = false; // 'INCH' or ?
            OriginLocation = Vector2.Origin;
            OriginTurn = 0;
            TopContour = new List<Vector2>(1024);
            BottomContour = new List<Vector2>(1024);
            Parts = new List<IPart>(1024);
            Pins = new List<IPin>(1024);
            Nails = new List<INail>(1024);
        }

        private void Update(ref State currentState, string data)
        {
            if (data.Length==0)
            {
                currentState = State.Idle;
                return;
            }
            switch (currentState)
            {
            case State.Idle:
            {
                var str = BoardUtil.SplitLine(data.ToLowerInvariant());
                switch (str[0])
                {
                case "brdout:":
                    currentState = State.Format;
                    break;
                case "nets:":
                    currentState = State.Nets;
                    netNames = new string[int.Parse(str[1])];
                    break;
                case "parts:":
                    currentState = State.Parts;
                    Parts.Capacity = int.Parse(str[1]);
                    break;
                case "pins:":
                    currentState = State.Pins;
                    Pins.Capacity = int.Parse(str[1]);
                    break;
                case "nails:":
                    currentState = State.Nails;
                    Nails.Capacity = int.Parse(str[1]);
                    break;
                default:
                    break; // leave in idle state
                }
                break;
            }
            case State.Format: // update contour
            {
                var str = BoardUtil.SplitLine(data);
                if (str.Length!=2)
                    throw new InvalidDataException();
                var newVert = new Vector2(int.Parse(str[0]), int.Parse(str[1]));
                if (TopContour.Count>0 && TopContour[TopContour.Count-1]==newVert)
                    break;
                TopContour.Add(newVert);
                break;
            }
            case State.Nets:
            {
                var str = BoardUtil.SplitLine(data);
                int id = int.Parse(str[0])-1;
                netNames[id] = str[1];
                break;
            }
            case State.Parts: // update partsTest
            {
                // 0     1    2   3    4    5    6
                // J3200 6765 430 9462 1386 8754 1
                // name  ?     ?   ?   ?    fp   side
                var str = BoardUtil.SplitLine(data);
                if (str.Length!=7)
                    throw new InvalidDataException();
                var part = new Part();
                part.Name = str[0];
                if (part.Name=="...")
                    part.Name = string.Empty;
                part.Side = BoardUtil.GetSide(int.Parse(str[6]));
                part.FirstPin = int.Parse(str[5]);
                if (prevPart!=null)
                    prevPart.PinCount = part.FirstPin-prevPart.FirstPin;
                Parts.Add(part);
                prevPart = part;
                break;
            }
            case State.Pins: // update pins
            {
                var str = BoardUtil.SplitLine(data); // X Y net side
                if (str.Length!=4)
                    throw new InvalidDataException();
                var pin = new Pin();
                pin.Location = new Vector2(int.Parse(str[0]), int.Parse(str[1]));
                pin.Net = int.Parse(str[2]);
                pin.Side = BoardUtil.GetSide(int.Parse(str[3]));
                if (pin.Side==(BoardSide.Top))
                    pin.Location = new Vector2(pin.Location.X, -pin.Location.Y);
                if (pin.Net==0)
                {
                    if (prevNet==0)
                        throw new InvalidDataException();
                    pin.Net = prevNet;
                }
                //pin.PartIndex = int.Parse(str[3]);
                pin.Name = netNames[pin.Net-1];
                Pins.Add(pin);
                prevNet = pin.Net;
                break;
            }
            case State.Nails: // update nails
            {
                // net, x, y, side?, name
                // id, x, y, net, side
                var str = BoardUtil.SplitLine(data);
                if (str.Length!=5)
                    throw new InvalidDataException();
                var nail = new Nail();
                nail.Id = int.Parse(str[0]);
                nail.Location = new Vector2(int.Parse(str[1]), int.Parse(str[2]));
                nail.Net = int.Parse(str[3]);
                nail.Side = BoardUtil.GetSide(int.Parse(str[4]));
                if (nail.Side==(BoardSide.Top))
                    nail.Location = new Vector2(nail.Location.X, -nail.Location.Y);
                nail.Name = netNames[nail.Net];
                if (nail.Name=="...")
                    nail.Name = string.Empty;
                Nails.Add(nail);
                break;
            }
            }
        }

        public bool Load(string path)
        {
            using (var r = new StreamReader(path, Encoding.GetEncoding("iso-8859-1")))
            {
                int lineIndex = 1;
                var state = State.Idle;
                while (!r.EndOfStream)
                {
                    try
                    {
                        var str = r.ReadLine();
                        Update(ref state, str);
                        lineIndex++;
                    }
                    catch (Exception e)
                    {
                        var fname = Path.GetFileName(path);
                        BoardUtil.OnUnknownFormat(fname, lineIndex);
                        return false;
                    }
                }
            }
            // fix pin count for last part
            if (Parts.Count>0)
            {
                var lastPart = Parts[Parts.Count-1];
                lastPart.PinCount = Pins.Count-lastPart.FirstPin;
            }
            // calculate contour bbox
            var cbox = Box2.Empty;
            foreach (var cv in TopContour)
                cbox.Merge(cv);
            // shift up all non-bottom stuff
            foreach (var pin in Pins)
            {
                var v = pin.Location;
                if (v.Y<cbox.Min.Y && pin.Side!=BoardSide.Bottom)
                {
                    v.Y += cbox.Height;
                    pin.Location = v;
                }
            }
            foreach (var nail in Nails)
            {
                var v = nail.Location;
                if (v.Y<cbox.Min.Y && nail.Side!=BoardSide.Bottom)
                {
                    v.Y += cbox.Height;
                    nail.Location = v;
                }
            }
            // flip bottom stuff
            foreach (var pin in Pins)
            {
                var v = pin.Location;
                if (pin.Side==BoardSide.Bottom)
                {
                    v.Y *= -1;
                    pin.Location = v;
                }
            }
            foreach (var nail in Nails)
            {
                var v = nail.Location;
                if (nail.Side==BoardSide.Bottom)
                {
                    v.Y *= -1;
                    nail.Location = v;
                }
            }
            // generate inverses for through-hole pins
            foreach (var pin in Pins)
            {
                if (pin.Side==BoardSide.Both)
                {
                    var inv = new Pin();                    
                    var v = pin.Location;
                    v.Y *= -1;
                    inv.Location = v;
                    inv.Net = pin.Net;
                    inv.Name = pin.Name;
                    inv.Side = BoardSide.Bottom;
                    inv.Inverse = pin;
                    pin.Side = BoardSide.Top;
                    pin.Inverse = inv;
                }
            }
            // rearrange mating pins
            foreach (var part in Parts)
            {
                for (int i = part.FirstPin; i<part.FirstPin+part.PinCount; i++)
                {
                    var pin = Pins[i];
                    if (part.Side!=pin.Side && pin.Inverse!=null)
                        Pins[i] = pin.Inverse; // swap Pins[i] and pin.Inverse
                }
            }
            // generate bottom contour
            for (int i = 0; i<TopContour.Count; i++)
            {
                var v = TopContour[i];
                v.Y *= -1;
                BottomContour.Add(v);
            }
            return true;
        }
    }

    internal class HonhanBdvFile : IBoardViewFile
    {
        public static bool Detect(string path)
        {
            try
            {
                var sb = new StringBuilder(1024);
                using (var r = new StreamReader(path, Encoding.GetEncoding("iso-8859-1")))
                {
                    var str = r.ReadLine();
                    Decode(str, sb, 1);
                    if (sb.ToString()=="<<format.asc>>")
                        return true;
                }
            }
            catch
            {}
            return false;
        }

        private static void Decode(string src, StringBuilder dst, int strIndex)
        {
            dst.Length = src.Length;
            int idx = strIndex % 127;
            for (int i = 0; i<src.Length; i++)
            {
                int s = (sbyte)src[i];
                if (s=='\n')
                    break;
                if (s <= 127)
                    dst[i] = (char)(byte)(127 - s + ' ' + idx);
                else
                    dst[i] = (char)(byte)(127 - s + ' ' - idx);
            }
        }

        private enum State
        {
            Idle,
            Format,
            Nails,
            Pins
        }
        
        private enum FormatState // '<<format.asc>>'
        {
            Idle,
            //License, // ' 11980-1        eM-Test Expert (R)     licence #1 AAAAAAAA TECHNOLOGY INC, '
            // ''
            //Desc, // ' Board Outline Contour             INCH units               5-Mar-2012 10:31'
            // ''
            //Origin, // ' User Datum  X  0.000,  Y  0.000,  Rotation   0.0'
            // ''
            //ContourHeader, // '      X           Y         Radius'
            // ''
            ContourVertex, // '   -0.030      0.385        0.000'
            // '' optional empty string
            //End // string beginning with '<<' met
        }

        private enum NailsState // <<nails.asc>>
        {
            Idle,
            //License,
            Desc, // 2-line section:
            // ' Test Fixture Nails      18/121  Selected Drills           5-Mar-2012 10:31'
            // '                         18 Nails,   18 Nets               INCH units'
            //NailHeader, // 'Nail         X         Y    Type Grid  T/B  Net   Net Name                         Virtual Pin/Via'
            NailEntry, // '$1         0.8200     0.1300   1  A1   (T)  #13   GND                                      V VIA .'
            //End // string beginning with '<<' met
        }

        private enum PinsState // <<pins.asc>>
        {
            Idle,
            //License,
            //Desc, // 2-line section:
            // ' Part Pins List          0/30   Selected Parts             5-Mar-2012 10:31'
            // '                                                           INCH units'
            //PreHeader, // 'Part        T/B'
            //PinHeader, // 'Pin   Name      X         Y     Layer  Net               Nail(s)'
            //PartSectionHeader, // 'Part U1     (T)'
            PartPinEntry, // '   1    1     0.1138     0.0701     1    MIC_GND                                   3'
            End // string beginning with '<<' met
        }

        public bool FlipBottomY { get { return true; } }
        public bool InchUnits { get; private set; }
        public Vector2 OriginLocation { get; private set; }
        public Angle OriginTurn { get; private set; }
        public List<Vector2> TopContour { get; private set; }
        public List<Vector2> BottomContour { get; private set; }
        public List<IPart> Parts { get; private set; }
        public List<IPin> Pins { get; private set; }
        public List<INail> Nails { get; private set; }
        private bool skipPart = false;

        public HonhanBdvFile()
        {
            InchUnits = false; // 'INCH' or ?
            OriginLocation = Vector2.Origin;
            OriginTurn = 0;
            TopContour = new List<Vector2>(1024);
            BottomContour = new List<Vector2>(1024);
            Parts = new List<IPart>(1024);
            Pins = new List<IPin>(1024);
            Nails = new List<INail>(1024);
        }

        private double ParseDouble(string s)
        { return double.Parse(s, CultureInfo.InvariantCulture); }

        private BoardSide GetSide(string code)
        {
            switch (code)
            {
            case "(T)":
                return BoardSide.Top;
            case "(B)":
                return BoardSide.Bottom;
            default:
                return BoardSide.Top;
            }
        }

        private void UpdateFormat(ref FormatState state, StringBuilder data)
        {
            char[] trimChars = {','};
            switch (state)
            {
            case FormatState.Idle:
            {
                var str = BoardUtil.SplitLine(data, 1);
                switch (str[0].ToLowerInvariant())
                {
                case "board":
                    if (data.ToString().Contains("INCH units"))
                        InchUnits = true;
                    break;
                case "user":
                    str = BoardUtil.SplitLine(data, 8);
                    var loc = new Vector2();
                    loc.X = ParseDouble(str[3].TrimEnd(trimChars));
                    loc.Y = ParseDouble(str[5].TrimEnd(trimChars));
                    OriginLocation = loc;
                    OriginTurn = ParseDouble(str[7].TrimEnd(trimChars));
                    break;
                case "x":
                    state = FormatState.ContourVertex;
                    break;
                }
                break;
            }
            case FormatState.ContourVertex:
            {
                // update contour
                var str = BoardUtil.SplitLine(data, 2);
                var newVert = new Vector2(ParseDouble(str[0]), ParseDouble(str[1]));
                if (TopContour.Count>0 && TopContour[TopContour.Count-1]==newVert)
                    break;
                TopContour.Add(newVert);
                break;
            }
            }
        }

        private void UpdateNails(ref NailsState state, StringBuilder data)
        {
            switch (state)
            {
            case NailsState.Idle:
                {
                    var str = BoardUtil.SplitLine(data, 1);
                    switch (str[0].ToLowerInvariant())
                    {
                    case "test":
                        state = NailsState.Desc;
                        break;
                    case "nail":
                        state = NailsState.NailEntry;
                        break;
                    }
                }
                break;
            case NailsState.Desc:
                //if (data.ToString().Contains("INCH units"))
                //    InchUnits = true;
                state = NailsState.Idle;
                break;
            case NailsState.NailEntry:
                {
                    var str = BoardUtil.SplitLine(data, 8);
                    var nail = new Nail();
                    nail.Id = int.Parse(str[0].Substring(1));
                    var loc = new Vector2();
                    loc.X = ParseDouble(str[1]);
                    loc.Y = ParseDouble(str[2]);
                    nail.Location = loc;
                    nail.Type = byte.Parse(str[3]);
                    nail.Side = GetSide(str[5]);
                    nail.Net = int.Parse(str[6].Substring(1));
                    nail.Name = str[7];
                    Nails.Add(nail);
                }
                break;
            }
        }

        private void UpdatePins(ref PinsState state, StringBuilder data)
        {
            if (data[0]!=' ')
                state = PinsState.Idle;
            switch (state)
            {
            case PinsState.Idle:
                {
                    if (data[0]==' ')
                        break;
                    var str = BoardUtil.SplitLine(data, 3);
                    if (str[2]==null)
                        break;
                    if (str[0]=="Part")
                    {
                        skipPart = false;
                        var part = new Part();
                        part.Name = str[1];
                        part.Side = GetSide(str[2]);
                        part.FirstPin = Pins.Count;
                        if (Parts.Count>0 && part.Name==Parts[Parts.Count-1].Name)
                        {
                            skipPart = true;
                            break;
                        }
                        Parts.Add(part);
                        state = PinsState.PartPinEntry;
                    }
                }
                break;
            // '   1    1     0.1138     0.0701     1    MIC_GND                                   3'
            case PinsState.PartPinEntry:
                if (!skipPart)
                {
                    var str = BoardUtil.SplitLine(data, 6);
                    var pin = new Pin();
                    //pin.PartIndex = Parts.Count-1;
                    var loc = new Vector2();
                    loc.X = ParseDouble(str[2]);
                    loc.Y = ParseDouble(str[3]);
                    pin.Location = loc;
                    pin.Name = str[5];
                    pin.Net = -1;
                    Pins.Add(pin);
                    Parts[Parts.Count-1].PinCount++;
                }
                break;
            }
        }

        private void Update(ref State currentState, ref FormatState fState,
            ref NailsState nState, ref PinsState pState, StringBuilder data)
        {
            if (data.Length==0 || data[0]=='\r')
            {
                //currentState = State.Idle;
                return;
            }
            if (data[0]=='<')
                currentState = State.Idle;
            switch (currentState)
            {
            case State.Idle:
                switch (data.ToString())
                {
                case "<<format.asc>>":
                    currentState = State.Format;
                    break;
                case "<<nails.asc>>":
                    currentState = State.Nails;
                    break;
                case "<<pins.asc>>":
                    currentState = State.Pins;
                    break;
                }
                break;
            case State.Format:
                UpdateFormat(ref fState, data);
                break;
            case State.Nails:
                UpdateNails(ref nState, data);
                break;
            case State.Pins:
                UpdatePins(ref pState, data);
                break;
            }
        }

        public bool Load(string path)
        {
            var sb = new StringBuilder(1024);
            using (var r = new StreamReader(path, Encoding.GetEncoding("iso-8859-1")))
            {
                var state = State.Idle;
                var fState = FormatState.Idle;
                var nState = NailsState.Idle;
                var pState = PinsState.Idle;
                int lineIndex = 1;
                while (!r.EndOfStream)
                {
                    try
                    {
                        var str = r.ReadLine();
                        Decode(str, sb, lineIndex++);
                        Update(ref state, ref fState, ref nState, ref pState, sb);
                    }
                    catch (Exception e)
                    {
                        var fname = Path.GetFileName(path);
                        BoardUtil.OnUnknownFormat(fname, lineIndex);
                        return false;
                    }
                }
            }
            // assign pin sides
            foreach (var part in Parts)
            {
                for (int i = part.FirstPin; i<part.FirstPin+part.PinCount; i++)
                    Pins[i].Side = part.Side;
            }
            // generate bottom contour
            for (int i = 0; i<TopContour.Count; i++)
            {
                var v = TopContour[i];
                v.Y *= -1;
                BottomContour.Add(v);
            }
            return true;
        }
    }
}
