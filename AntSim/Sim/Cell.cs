using System;

namespace AntSim.Sim
{
    public record Cell(Grid Grid, int X, int Y)
    {
        private Creature _creature;

        public event EventHandler<Creature> CreatureAdded;
        public event EventHandler<Creature> CreatureRemoved;

        public int FoodAmount { get; set; }
        public bool IsWall { get; init; }

        public bool CanCreatureMoveHere => _creature == null && !IsWall;

        public Creature Creature
        {
            get => _creature;
            set
            {
                /*
                 *      creature    value   result
                 * ----------------------------------------------------
                 * 1.   NULL        NULL            (nothing)
                 * 2.   A           A               (nothing)
                 * 3.   NULL        A       ADD A
                 * 4.   A           B       ERROR
                 * 5.   A           NULL    REMOVE A
                 */

                // 1/2
                if (ReferenceEquals(_creature, value))
                {
                    return;
                }

                // 3
                if (_creature == null)
                {
                    _creature = value;
                    OnCreatureAdded(value);
                    return;
                }

                // 4
                if (value != null)
                {
                    throw new InvalidOperationException("Trying to add creature where one is already present.");
                }

                // 5
                var removed = _creature;
                _creature = null;
                OnCreatureRemoved(removed);
            }
        }

        protected virtual void OnCreatureAdded(Creature c) => CreatureAdded?.Invoke(this, c);
        protected virtual void OnCreatureRemoved(Creature c) => CreatureRemoved?.Invoke(this, c);
    }
}