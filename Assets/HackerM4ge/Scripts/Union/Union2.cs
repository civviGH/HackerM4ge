using System;

public abstract class Union2<A, B>
{
    public abstract T Match<T>(Func<A, T> f, Func<B, T> g);
    // private ctor ensures no external classes can inherit
    private Union2() { }

    public sealed class Case1 : Union2<A, B>
    {
        public readonly A Item;
        public Case1(A item) : base() { this.Item = item; }
        public override T Match<T>(Func<A, T> f, Func<B, T> g)
        {
            return f(Item);
        }
    }

    public sealed class Case2 : Union2<A, B>
    {
        public readonly B Item;
        public Case2(B item) { this.Item = item; }
        public override T Match<T>(Func<A, T> f, Func<B, T> g)
        {
            return g(Item);
        }
    }
}