using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuLivePlayer.Model;

namespace OsuLivePlayer.Util
{
    public static class Easing
    {
        public static double Reverse(Func<double, double> function, double value) => 1 - function(1 - value);
        public static double ToInOut(Func<double, double> function, double value) =>
            .5 * (value < .5 ? function(2 * value) : (2 - function(2 - 2 * value)));

        public static readonly Func<double, double> Step = x => x >= 1 ? 1 : 0;
        public static readonly Func<double, double> Linear = x => x;

        public static readonly Func<double, double> QuadIn = x => x * x;
        public static readonly Func<double, double> QuadOut = x => Reverse(QuadIn, x);
        public static readonly Func<double, double> QuadInOut = x => ToInOut(QuadIn, x);
        public static readonly Func<double, double> CubicIn = x => x * x * x;
        public static readonly Func<double, double> CubicOut = x => Reverse(CubicIn, x);
        public static readonly Func<double, double> CubicInOut = x => ToInOut(CubicIn, x);
        public static readonly Func<double, double> QuartIn = x => x * x * x * x;
        public static readonly Func<double, double> QuartOut = x => Reverse(QuartIn, x);
        public static readonly Func<double, double> QuartInOut = x => ToInOut(QuartIn, x);
        public static readonly Func<double, double> QuintIn = x => x * x * x * x * x;
        public static readonly Func<double, double> QuintOut = x => Reverse(QuintIn, x);
        public static readonly Func<double, double> QuintInOut = x => ToInOut(QuintIn, x);

        public static readonly Func<double, double> SineIn = x => 1 - Math.Cos(x * Math.PI / 2);
        public static readonly Func<double, double> SineOut = x => Reverse(SineIn, x);
        public static readonly Func<double, double> SineInOut = x => ToInOut(SineIn, x);

        public static readonly Func<double, double> ExpoIn = x => Math.Pow(2, 10 * (x - 1));
        public static readonly Func<double, double> ExpoOut = x => Reverse(ExpoIn, x);
        public static readonly Func<double, double> ExpoInOut = x => ToInOut(ExpoIn, x);

        public static readonly Func<double, double> CircIn = x => 1 - Math.Sqrt(1 - x * x);
        public static readonly Func<double, double> CircOut = x => Reverse(CircIn, x);
        public static readonly Func<double, double> CircInOut = x => ToInOut(CircIn, x);

        public static readonly Func<double, double> BackIn = x => x * x * ((1.70158 + 1) * x - 1.70158);
        public static readonly Func<double, double> BackOut = x => Reverse(BackIn, x);
        public static readonly Func<double, double> BackInOut = x =>
            ToInOut((y) => y * y * ((1.70158 * 1.525 + 1) * y - 1.70158 * 1.525), x);

        public static readonly Func<double, double> BounceIn = x => Reverse(BounceOut, x);
        public static readonly Func<double, double> BounceOut = x =>
            x < 1 / 2.75 ? 7.5625 * x * x :
            x < 2 / 2.75 ? 7.5625 * (x -= (1.5 / 2.75)) * x + .75 :
            x < 2.5 / 2.75 ? 7.5625 * (x -= (2.25 / 2.75)) * x + .9375 : 7.5625 * (x -= (2.625 / 2.75)) * x + .984375;
        public static readonly Func<double, double> BounceInOut = x => ToInOut(BounceIn, x);

        public static readonly Func<double, double> ElasticIn = x => Reverse(ElasticOut, x);
        public static readonly Func<double, double> ElasticOut = x =>
            Math.Pow(2, -10 * x) * Math.Sin((x - 0.075) * (2 * Math.PI) / .3) + 1;
        public static readonly Func<double, double> ElasticOutHalf =
            x => Math.Pow(2, -10 * x) * Math.Sin((0.5 * x - 0.075) * (2 * Math.PI) / .3) + 1;
        public static readonly Func<double, double> ElasticOutQuarter =
            x => Math.Pow(2, -10 * x) * Math.Sin((0.25 * x - 0.075) * (2 * Math.PI) / .3) + 1;
        public static readonly Func<double, double> ElasticInOut = x => ToInOut(ElasticIn, x);

        public static double Ease(this EasingEnum easingEnum, double value)
            => easingEnum.ToEasingFunction().Invoke(value);

        public static Func<double, double> ToEasingFunction(this EasingEnum easingEnum)
        {
            switch (easingEnum)
            {
                default:
                case EasingEnum.Linear: return Linear;

                case EasingEnum.EasingIn:
                case EasingEnum.QuadIn: return QuadIn;
                case EasingEnum.EasingOut:
                case EasingEnum.QuadOut: return QuadOut;
                case EasingEnum.QuadInOut: return QuadInOut;

                case EasingEnum.CubicIn: return CubicIn;
                case EasingEnum.CubicOut: return CubicOut;
                case EasingEnum.CubicInOut: return CubicInOut;
                case EasingEnum.QuartIn: return QuartIn;
                case EasingEnum.QuartOut: return QuartOut;
                case EasingEnum.QuartInOut: return QuartInOut;
                case EasingEnum.QuintIn: return QuintIn;
                case EasingEnum.QuintOut: return QuintOut;
                case EasingEnum.QuintInOut: return QuintInOut;

                case EasingEnum.SineIn: return SineIn;
                case EasingEnum.SineOut: return SineOut;
                case EasingEnum.SineInOut: return SineInOut;
                case EasingEnum.ExpoIn: return ExpoIn;
                case EasingEnum.ExpoOut: return ExpoOut;
                case EasingEnum.ExpoInOut: return ExpoInOut;
                case EasingEnum.CircIn: return CircIn;
                case EasingEnum.CircOut: return CircOut;
                case EasingEnum.CircInOut: return CircInOut;
                case EasingEnum.ElasticIn: return ElasticIn;
                case EasingEnum.ElasticOut: return ElasticOut;
                case EasingEnum.ElasticHalfOut: return ElasticOutHalf;
                case EasingEnum.ElasticQuarterOut: return ElasticOutQuarter;
                case EasingEnum.ElasticInOut: return ElasticInOut;
                case EasingEnum.BackIn: return BackIn;
                case EasingEnum.BackOut: return BackOut;
                case EasingEnum.BackInOut: return BackInOut;
                case EasingEnum.BounceIn: return BounceIn;
                case EasingEnum.BounceOut: return BounceOut;
                case EasingEnum.BounceInOut: return BounceInOut;
            }
        }
    }
}
