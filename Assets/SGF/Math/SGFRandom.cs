////////////////////////////////////////////////////////////////////
//                            _ooOoo_                             //
//                           o8888888o                            //
//                           88" . "88                            //
//                           (| ^_^ |)                            //
//                           O\  =  /O                            //
//                        ____/`---'\____                         //
//                      .'  \\|     |//  `.                       //
//                     /  \\|||  :  |||//  \                      //
//                    /  _||||| -:- |||||-  \                     //
//                    |   | \\\  -  /// |   |                     //
//                    | \_|  ''\---/''  |   |                     //
//                    \  .-\__  `-`  ___/-. /                     //
//                  ___`. .'  /--.--\  `. . ___                   //
//                ."" '<  `.___\_<|>_/___.'  >'"".                //
//              | | :  `- \`.;`\ _ /`;.`/ - ` : | |               //
//              \  \ `-.   \_ __\ /__ _/   .-` /  /               //
//        ========`-.____`-.___\_____/___.-`____.-'========       //
//                             `=---='                            //
//        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^      //
//            佛祖保佑       无BUG        不修改                   //
////////////////////////////////////////////////////////////////////
/*
* 描述：
* 作者：slicol
*/

namespace SGF
{
    public class SGFRandom
    {
        #region 默认实例
        public static SGFRandom Default = new SGFRandom();
        #endregion

        #region 线性同余参数

        //线性同余随机数生成算法
        private const int PrimeA = 214013;
        private const int PrimeB = 2531011;        

        #endregion

        //归一化
        private const float Mask15Bit_1 = 1.0f / 0x7fff;
        private const int Mask15Bit = 0x7fff;

        private int m_Value = 0;

        public int Seed
        {
            set { m_Value = value; }
            get { return m_Value; }
        }

        /// <summary>
        /// 采用线性同余算法产生一个0~1之间的随机小数
        /// </summary>
        /// <returns></returns>
        public float Rnd()
        {
            float val = ((((m_Value = m_Value * PrimeA + PrimeB) >> 16) & Mask15Bit) - 1) * Mask15Bit_1;
            return (val > 0.99999f ? 0.99999f : val);
        }

        public float Range(float min, float max)
        {
            return min + Rnd() * (max - min);
        }

        public int Range(int min, int max)
        {
            return (int)(min + Rnd() * (max - min));
        }


        public float Range(float max)
        {
            return Range(0, max);
        }

        public int Range(int max)
        {
            return Range(0, max);
        }



    }
}
