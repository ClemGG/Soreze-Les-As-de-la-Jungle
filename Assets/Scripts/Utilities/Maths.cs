namespace Clement.Utilities.Maths
{

    public static class Maths
    {

        /// <summary>
        /// Pour changer une valeur d'un intervalle à un autre
        /// </summary>
        /// <param name="f"></param>
        /// <param name="ancienX"></param>
        /// <param name="ancienY"></param>
        /// <param name="nouveauX"></param>
        /// <param name="nouveauY"></param>
        /// <returns></returns>
        public static float map(this float f, float ancienX, float ancienY, float nouveauX, float nouveauY)
        {
            return nouveauX + (f - ancienX) * (nouveauY - nouveauX) / (ancienY - ancienX);
        }



        /// <summary>
        /// Pareil que le Mathf.Approximately, mais celui-là permet d'utiliser un intervalle de sensibilité. Utile quand on ne sait pas si a sera plus grand ou plus petit que b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool FastApproximate(float a, float b, float threshold)
        {
            return ((a < b) ? (b - a) : (a - b)) <= threshold;
        }


        /// <summary>
        /// Similaire à FastApproximately, mais ne s'utilise que si a est strictement plus grand que b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool FastApproximatelyWithFirstArgumentAsSuperiorStrict(float a, float b, float threshold)
        {
            return (a - b) <= threshold;
        }


        /// <summary>
        /// Similaire à FastApproximately, mais ne s'utilise que si a est strictement plus petit que b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool FastApproximatelyWithFirstArgumentAsInferiorStrict(float a, float b, float threshold)
        {
            return (b - a) <= threshold;
        }
    }
}