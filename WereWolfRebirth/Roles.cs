using System;
using System.Collections.Generic;
using System.Text;

namespace WereWolfRebirth.Roles
{
    class Citizien : IPlayer
    {
        public bool HasWon() => throw new NotImplementedException();
    }

    class Witch : Citizien
    {
        public new bool HasWon() => base.HasWon();
    }

    class LittleGirl : Citizien
    {
        public new bool HasWon() => base.HasWon();
    }

    class Hunter : Citizien
    {
        public new bool HasWon() => base.HasWon();
    }

    class Cupidon : Citizien
    {
        public new bool HasWon() => base.HasWon();
    }

    class Seer : Citizien
    {
        public new bool HasWon() => base.HasWon();
    }

    class TalkativeSeer : Seer
    {
        public new bool HasWon() => base.HasWon();
    }

    interface IPlayer
    {
        bool HasWon();
    }
}
