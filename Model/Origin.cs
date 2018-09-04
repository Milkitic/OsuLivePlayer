using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuLivePlayer.Model
{

    internal struct Origin
    {
        public int? X { get; private set; }
        public int? Y { get; private set; }
        public OriginEnum Enum { get; private set; }

        public Origin(int x, int y)
        {
            X = x;
            Y = y;
            Enum = OriginEnum.Free;
        }

        public static Origin Default => new Origin { X = null, Y = null, Enum = OriginEnum.Centre };
        public static Origin BottomLeft => new Origin { X = null, Y = null, Enum = OriginEnum.BottomLeft };
        public static Origin BottomCentre => new Origin { X = null, Y = null, Enum = OriginEnum.BottomCentre };
        public static Origin BottomRight => new Origin { X = null, Y = null, Enum = OriginEnum.BottomRight };
        public static Origin CentreLeft => new Origin { X = null, Y = null, Enum = OriginEnum.CentreLeft };
        public static Origin Centre => new Origin { X = null, Y = null, Enum = OriginEnum.Centre };
        public static Origin CentreRight => new Origin { X = null, Y = null, Enum = OriginEnum.CentreRight };
        public static Origin TopLeft => new Origin { X = null, Y = null, Enum = OriginEnum.TopLeft };
        public static Origin TopCentre => new Origin { X = null, Y = null, Enum = OriginEnum.TopCentre };
        public static Origin TopRight => new Origin { X = null, Y = null, Enum = OriginEnum.TopRight };

        public enum OriginEnum
        {
            Free = 0,
            BottomLeft,
            BottomCentre,
            BottomRight,
            CentreLeft,
            Centre,
            CentreRight,
            TopLeft,
            TopCentre,
            TopRight,
        }
    }

}
