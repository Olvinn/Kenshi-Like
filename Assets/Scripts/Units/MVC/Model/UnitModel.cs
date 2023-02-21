using System;
using Units.Structures;
using UnityEngine;

namespace Units.MVC.Model
{
    [Serializable]
    public class UnitModel
    {
        public event Action<int> onHPChanged;
        public bool isDead => _cur.healthPoints <= 0;
        
        private UnitStats _cur, _def; //current and default unit stats
        private UnitAppearance _app; //how this unit actually looks like

        private Vector3 _pos;
        
        public UnitModel(UnitStats stats, UnitAppearance appearance)
        {
            _app = appearance;
            _cur = _def = stats;
        }

        public UnitStats GetStats()
        {
            return _cur;
        }

        public UnitAppearance GetAppearance()
        {
            return _app;
        }

        public void GetDamage(int value)
        {
            if (isDead || value <= 0)
                return;
            int old = _cur.healthPoints;
            if (_cur.healthPoints >= value)
                _cur.healthPoints -= value;
            else
                _cur.healthPoints = 0;
            onHPChanged?.Invoke(_cur.healthPoints - old);
        }

        public void GetHealed(int value)
        {
            if (isDead || value <= 0)
                return;
            int old = _cur.healthPoints;
            if (_def.healthPoints >= value + _cur.healthPoints)
                _cur.healthPoints += value;
            else
                _cur.healthPoints = _def.healthPoints;
            onHPChanged?.Invoke(_cur.healthPoints - old);
        }

        public void GetRevived()
        {
            if (!isDead)
                return;
            
            _cur.healthPoints = 1;
            onHPChanged?.Invoke(1);
        }

        public void UpdatePosition(Vector3 pos)
        {
            _pos = pos;
        }

        public Vector3 GetPosition()
        {
            return _pos;
        }
    }
}
