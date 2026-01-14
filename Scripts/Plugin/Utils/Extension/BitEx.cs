
namespace Utils.Extension
{
    public static class BitEx
    {
        public static bool EqualBit(this int bit, int num)
        {
            return (bit & (1 << num)) == (1 << num);
        }

        public static bool EqualBit(this ulong bit, int num)
        {
            return (bit & (1uL << num)) > 0;
        }

        // bit 에서 a ~ b 까지의 정보를 가져오기
        public static ulong GetBitValue(this ulong bit, int a, int b)
        {
            ulong value = AllBitMast(a, b);
            return BitIntersection(bit, value) >> a;
        }

        // a 까지 모두 1로
        public static ulong AllBitMast(int a)
        {
            //64비트일 때 예외처리
            if (a == 64)
                return ~0uL;
            return (1uL << a) - 1;
        }

        // a 에서 b 까지 모두 1로
        public static ulong AllBitMast(int a, int b)
        {
            ulong abit = AllBitMast(a);
            ulong total = AllBitMast(b);

            return BitDifference(total, abit);
        }

        // 합집합
        public static ulong BitUnion(ulong a, ulong b)
        {
            return a | b;
        }

        // 차집합
        public static ulong BitDifference(ulong a, ulong b)
        {
            return a & ~b;
        }

        // 교집합
        public static ulong BitIntersection(ulong a, ulong b)
        {
            return a & b;
        }
    }
}
