using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timeline;

namespace Finance
{
    public class Journal<Time>
        : ITimeline<Time, List<Transaction>>
        where Time : notnull, IComparable<Time>
    {
        private readonly DictionaryTimeline<Time, List<Transaction>> _timeline;

        public Journal()
        {
            _timeline = new();
        }

        public IEnumerable<List<Transaction>> Values
        {
            get
            {
                foreach (var entry in _timeline)
                {
                    yield return entry.Event;
                }
            }
        }

        public List<Transaction> this[Time time]
        {
            get
            {
                return _timeline[time];
            }
            set
            {
                _timeline[time] = value;
            }
        }

        public virtual void AddTransaction(Time time, Transaction transaction)
        {
            if (!_timeline.HasEvent(time))
            {
                _timeline[time] = new();
            }

            _timeline[time].Add(transaction);
        }

        public void Add(Journal<Time> journal)
        {
            foreach (var entry in journal)
            {
                foreach (var transaction in entry.Event)
                {
                    AddTransaction(entry.Time, transaction);
                }
            }
        }

        public override string? ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Journal:");

            foreach (var point in _timeline)
            {
                builder.AppendLine($"{point.Time}");

                foreach (var transaction in point.Event)
                {
                    builder.AppendLine(transaction.ToString());
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _timeline.GetEnumerator();
        }

        public bool HasEvent(Time time)
        {
            return _timeline.HasEvent(time);
        }

        public IEnumerator<Point<Time, List<Transaction>>> GetEnumerator()
        {
            return _timeline.GetEnumerator();
        }

        public Point<Time, List<Transaction>> MostRecent(Time time)
        {
            return _timeline.MostRecent(time);
        }

        public Point<Time, List<Transaction>> First()
        {
            return _timeline.First();
        }
    }
}
