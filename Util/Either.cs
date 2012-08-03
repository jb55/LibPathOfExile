using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathOfExile.Util
{
    public struct Either<L, R>
    {
        private L _left;
        private R _right;

        public L Left {
            get {
                return _left;
            }
            set {
                _isRight = false;
                _left = value;
            }
        }

        public R Right {
            get {
                return _right;
            }
            set {
                _isRight = true;
                _right = value;
            }
        }

        private bool _isRight;
        public bool IsRight { 
            get {
                return _isRight;
            }
        }

        public bool IsLeft { 
            get {
                return !IsRight;
            } 
        }

        public static Either<L, R> MakeLeft(L data) {
            return new Either<L, R> { Left = data };
        }

        public static Either<L, R> MakeRight(R data) {
            return new Either<L, R> { Right = data };
        }

    }

    public static class EitherMixins
    {
        public static Either<L, R2> Select<L, R, R2>(this Either<L, R> e, Func<R, R2> fn) {
            return new Either<L, R2> { Left = e.Left, Right = fn(e.Right) };
        }

        public static Either<L, R2> SelectMany<L, R, R2>(this Either<L, R> e, Func<R, Either<L, R2>> fn) {
            if (e.IsLeft)
              return Either<L,R2>.MakeLeft(e.Left);
            return fn(e.Right);
        }

        public static Either<L, R2> SelectMany<L, R, U, R2>(this Either<L, R> e, Func<R, Either<L, U>> k, Func<R, U, R2> s) {
            return e.SelectMany(x => k(x).SelectMany(y => s(x, y).ToRight<L, R2>()));
        }

        public static void Run<L, R>(this Either<L, R> e, Action<R> fn) {
          if (e.IsRight)
            fn(e.Right);
        }

        public static R FromEither<L, R>(this Either<L, R> e, Func<Exception> ex) {
          if (e.IsRight)
            return e.Right;
          else
            throw ex();
        }

        public static R FromEither<L, R>(this Either<L, R> e, Func<R> def) {
          if (e.IsRight)
            return e.Right;
          else
            return def();
        }

        public static Either<L, R> As<L, T, R>(this Either<L, T> a, Func<L> onFail) where R : class {
          return a.SelectMany(r => {
            var t = a.Right as R;
            if (t != null)
              return t.ToRight<L, R>();
            else
              return onFail().ToLeft<L, R>();
          });
        }

        public static IEnumerable<L> SelectInvalid<L, R>(this IEnumerable<Either<L, R>> e) {
            return e.Where(x => x.IsLeft).Select(x => x.Left);
        }

        public static IEnumerable<R> SelectValid<L, R>(this IEnumerable<Either<L, R>> e) {
            return e.Where(x => x.IsRight).Select(x => x.Right);
        }

        public static Either<L, R> ToRight<L, R>(this R data) {
            return Either<L, R>.MakeRight(data);
        }

        public static Either<L, R> ToLeft<L, R>(this L data) {
            return Either<L, R>.MakeLeft(data);
        }

    }
}
