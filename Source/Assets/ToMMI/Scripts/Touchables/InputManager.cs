/*
 * @author Francesco Strada
 */

using System.Collections.Generic;
using System.Linq;
using Touchables.MultiTouchManager;
using Touchables.TokenEngine;

namespace Touchables
{
    public static class InputManager
    {
        private static SortedDictionary<int, FingerTouch> _touches = new SortedDictionary<int, FingerTouch>();
        private static SortedDictionary<int, Token> _tokens = new SortedDictionary<int, Token>();

        public static List<FingerTouch> Touches
        {
            get
            {
                return _touches.Values.ToList();
            }
        }

        public static FingerTouch GetTouch(int id)
        {
            return _touches[id];
        }

        public static List<Token> Tokens
        {
            get
            {
                return _tokens.Values.ToList();
            }
        }

        public static void AddToken(Token t)
        {
            _tokens[t.Id] = t;
        }

        public static Token GetToken(int id)
        {
            return _tokens[id];
        }

        public static void RemoveToken(int id)
        {
            if (_tokens.ContainsKey(id))
                _tokens.Remove(id);
        }

        public static int FingersCount()
        {
            return _touches.Count;
        }

        internal static void UpdateFingersCancelled()
        {
            HashSet<int> toRemovePointIndexes = new HashSet<int>();

            foreach (FingerTouch touch in _touches.Values)
            {
                if (touch.State == TouchState.Ended)
                    toRemovePointIndexes.Add(touch.Id);
            }
            //This is BAD => double iteration
            foreach (int index in toRemovePointIndexes)
            {
                _touches.Remove(index);
            }
        }


        internal static void AddFingerTouch(TouchInput t)
        {
            FingerTouch finger;
            if (_touches.TryGetValue(t.Id, out finger))
            {
                finger.Update(t);
                _touches[t.Id] = finger;
            }
            else
                _touches[t.Id] = new FingerTouch(t);
        }

        internal static void RemoveFingerTouch(int id)
        {
            if (_touches.ContainsKey(id))
                _touches.Remove(id);
        }

        internal static void SetFingersCancelled(int[] indexes)
        {
            FingerTouch finger;
            for (int i = 0; i < indexes.Length; i++)
            {
                if (_touches.TryGetValue(indexes[i], out finger))
                {
                    finger.SetState(TouchState.Ended);
                }
            }
        }

        internal static void AddToken(InternalToken token)
        {
            _tokens[token.Id] = new Token(token,TokenManager.Instance.ContinuousMeanSquare);
        }


    }
}
