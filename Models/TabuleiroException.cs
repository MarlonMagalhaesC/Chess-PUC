using System;
using ChessAPI.Models;

namespace ChessAPI.Models
{
    public class TabuleiroException : Exception {

        public TabuleiroException(string msg) : base(msg) {
        }
    }
}
