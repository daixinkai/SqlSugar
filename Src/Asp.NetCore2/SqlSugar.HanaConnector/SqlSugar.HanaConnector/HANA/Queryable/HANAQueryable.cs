﻿namespace SqlSugar.HANAConnector
{
    public class HANAQueryable<T> : QueryableProvider<T>
    {
        public override ISugarQueryable<T> With(string withString)
        {
            return this;
        }

        public override ISugarQueryable<T> PartitionBy(string groupFileds)
        {
            this.GroupBy(groupFileds);
            return this;
        }
    }
    public class HANAQueryable<T, T2> : QueryableProvider<T, T2>
    {

    }
    public class HANAQueryable<T, T2, T3> : QueryableProvider<T, T2, T3>
    {

    }
    public class HANAQueryable<T, T2, T3, T4> : QueryableProvider<T, T2, T3, T4>
    {

    }
    public class HANAQueryable<T, T2, T3, T4, T5> : QueryableProvider<T, T2, T3, T4, T5>
    {

    }
    public class HANAQueryable<T, T2, T3, T4, T5, T6> : QueryableProvider<T, T2, T3, T4, T5, T6>
    {

    }
    public class HANAQueryable<T, T2, T3, T4, T5, T6, T7> : QueryableProvider<T, T2, T3, T4, T5, T6, T7>
    {

    }
    public class HANAQueryable<T, T2, T3, T4, T5, T6, T7, T8> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8>
    {

    }
    public class HANAQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8,T9>
    {

    }
    public class HANAQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9,T10> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {

    }
    public class HANAQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8 ,T9, T10, T11>
    {

    }
    public class HANAQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {

    }
}
