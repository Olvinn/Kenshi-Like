using Data;
using UnityEngine;

namespace Units.Commands.SimpleCommands
{
    public class MoveCommand : Command
    {
        public override CommandType Type => CommandType.Move;
        public override string CommandName => "Moving"; 
        
        private Vector3 _pos;
        private Transform _targetTr;
        private float _stopDist;
        private Vector3 _offset;

        public MoveCommand(Vector3 pos, Vector3 offset, float stoppingDistance, bool isDirectCommand)
        {
            _pos = pos;
            _stopDist = stoppingDistance;
            IsDirectCommand = isDirectCommand;
            _offset = offset;
        }

        public MoveCommand(Transform tr, Vector3 offset, float stoppingDistance, bool isDirectCommand)
        {
            _pos = tr.position;
            _targetTr = tr;
            _stopDist = stoppingDistance;
            IsDirectCommand = isDirectCommand;
            _offset = offset;
        }

        public override void Update()
        {
            if (!IsRunning)
                return;
            
            base.Update();
            
            Vector3 temp = Vector3.zero;
            if (_targetTr != null)
            {
                _pos = _targetTr.position;
                temp = _pos + (Vector3)(_targetTr.localToWorldMatrix * _offset);
            }
            else
                temp = _pos + _offset;

            if (Vector3.Distance(Executor.Position, temp) < _stopDist)
                Done();
            else
                Executor.MoveTo(temp, _stopDist);
        }
        
        public override bool Equals(object obj)
        {
            var c = obj as MoveCommand;
            if (c == null)
                return false;
            if (Executor != null && Executor.Equals(c.Executor) && c._pos == _pos)
                return true;
            return false;
        }
    }
}
