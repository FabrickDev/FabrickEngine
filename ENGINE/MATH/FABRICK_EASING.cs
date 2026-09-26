/*
References:
https://easings.net/
*/

namespace Fabrick.ENGINE.MATH
{
    public class EASING
    {
        public enum EasingType
        {
            EaseInSine,
            EaseOutSine,
            EaseInOutSine,
            EaseInQuad,
            EaseOutQuad,
            EaseInOutQuad,
            EaseInCubic,
            EaseOutCubic,
            EaseInOutCubic,
            EaseInQuart,
            EaseOutQuart,
            EaseInOutQuart,
            EaseInQuint,
            EaseOutQuint,
            EaseInOutQuint,
            EaseInExpo,
            EaseOutExpo,
            EaseInOutExpo,
            EaseInCirc,
            EaseOutCirc,
            EaseInOutCirc,
            EaseInBack,
            EaseOutBack,
            EaseInOutBack,
            EaseInElastic,
            EaseOutElastic,
            EaseInOutElastic,
            EaseInBounce,
            EaseOutBounce,
            EaseInOutBounce
        }
        public static double Ease(EasingType type, double state)
        {
            //Constant variables
            double c1 = 1.70158;
            double c2 = c1 * 1.525;
            double c3 = c1 + 1;
            double c4 = (2 * Math.PI) / 3;
            double c5 = (2 * Math.PI) / 4.5;
            double n1 = 7.5625;
            double d1 = 2.75;

            switch (type)
            {
                case EasingType.EaseInSine:
                    return 1 - Math.Cos((state * Math.PI) / 2);
                case EasingType.EaseOutSine:
                    return Math.Sin((state * Math.PI) / 2);
                case EasingType.EaseInOutSine:
                    return -(Math.Cos(Math.PI * state) - 1) / 2;

                case EasingType.EaseInQuad:
                    return state * state;
                case EasingType.EaseOutQuad:
                    return 1 - (1 - state) * (1 - state);
                case EasingType.EaseInOutQuad:
                    return state < 0.5 ? 2 * state * state : 1 - Math.Pow(-2 * state + 2, 2) / 2;

                case EasingType.EaseInCubic:
                    return state * state * state;
                case EasingType.EaseOutCubic:
                    return 1 - Math.Pow(1 - state, 3);
                case EasingType.EaseInOutCubic:
                    return state < 0.5 ? 4 * state * state * state : 1 - Math.Pow(-2 * state + 2, 3) / 2;

                case EasingType.EaseInQuart:
                    return state * state * state * state;
                case EasingType.EaseOutQuart:
                    return 1 - Math.Pow(1 - state, 4);
                case EasingType.EaseInOutQuart:
                    return state < 0.5 ? 8 * state * state * state * state : 1 - Math.Pow(-2 * state + 2, 4) / 2;

                case EasingType.EaseInQuint:
                    return state * state * state * state * state;
                case EasingType.EaseOutQuint:
                    return 1 - Math.Pow(1 - state, 5);
                case EasingType.EaseInOutQuint:
                    return state < 0.5 ? 16 * state * state * state * state * state : 1 - Math.Pow(-2 * state + 2, 5) / 2;

                case EasingType.EaseInExpo:
                    return state == 0 ? 0 : Math.Pow(2, 10 * state - 10);
                case EasingType.EaseOutExpo:
                    return state == 1 ? 1 : 1 - Math.Pow(2, -10 * state);
                case EasingType.EaseInOutExpo:
                    return state == 0 ? 0 : state == 1 ? 1 : state < 0.5 ? Math.Pow(2, 20 * state - 10) / 2 : (2 - Math.Pow(2, -20 * state + 10)) / 2;

                case EasingType.EaseInCirc:
                    return 1 - Math.Sqrt(1 - Math.Pow(state, 2));
                case EasingType.EaseOutCirc:
                    return Math.Sqrt(1 - Math.Pow(state - 1, 2));
                case EasingType.EaseInOutCirc:
                    return state < 0.5 ? (1 - Math.Sqrt(1 - Math.Pow(2 * state, 2))) / 2 : (Math.Sqrt(1 - Math.Pow(-2 * state + 2, 2)) + 1) / 2;

                case EasingType.EaseInBack:
                    return c3 * state * state * state - c1 * state * state;
                case EasingType.EaseOutBack:
                    return 1 + c3 * Math.Pow(state - 1, 3) + c1 * Math.Pow(state - 1, 2);
                case EasingType.EaseInOutBack:
                    return state < 0.5 ? (Math.Pow(2 * state, 2) * ((c2 + 1) * 2 * state - c2)) / 2 : (Math.Pow(2 * state - 2, 2) * ((c2 + 1) * (state * 2 - 2) + c2) + 2) / 2;

                case EasingType.EaseInElastic:
                    return state == 0 ? 0 : state == 1 ? 1 : -Math.Pow(2, 10 * state - 10) * Math.Sin((state * 10 - 10.75) * c4);
                case EasingType.EaseOutElastic:
                    return state == 0  ? 0  : state == 1  ? 1  : Math.Pow(2, -10 * state) * Math.Sin((state * 10 - 0.75) * c4) + 1;
                case EasingType.EaseInOutElastic:
                    return state == 0  ? 0  : state == 1  ? 1  : state < 0.5  ? -(Math.Pow(2, 20 * state - 10) * Math.Sin((20 * state - 11.125) * c5)) / 2  : (Math.Pow(2, -20 * state + 10) * Math.Sin((20 * state - 11.125) * c5)) / 2 + 1;

                case EasingType.EaseInBounce:
                    return 1 - Ease(EasingType.EaseOutBounce, 1 - state);
                case EasingType.EaseOutBounce:
                    if (state < 1 / d1) {
                        return n1 * state * state;
                    } else if (state < 2 / d1) {
                        return n1 * (state -= 1.5 / d1) * state + 0.75;
                    } else if (state < 2.5 / d1) {
                        return n1 * (state -= 2.25 / d1) * state + 0.9375;
                    } else {
                        return n1 * (state -= 2.625 / d1) * state + 0.984375;
                    }
                case EasingType.EaseInOutBounce:
                    return state < 0.5 ? (1 - Ease(EasingType.EaseOutBounce, 1 - 2 * state)) / 2 : (1 + Ease(EasingType.EaseOutBounce, 2 * state - 1)) / 2;
                default:
                    return 0;
            }
        }
    }
}