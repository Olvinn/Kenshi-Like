using System;
using Units.Structures;
using UnityEngine;

namespace Units.MVC.Model
{
    [Serializable]
    public class UnitModel
    {
        //events for controller notification
        public Action<int> onHPChanged;
        public Action<Vector3> onPositionChanged;
        public bool isDead => _currentStats.healthPoints <= 0;
        
        [SerializeField] private UnitStats _currentStats, _defaultStats;
        [SerializeField] private UnitAppearance _appearance; //how this unit actually looks like

        [SerializeField] private Vector3 _position;

        #region SetUp
        public UnitModel(UnitStats stats, UnitAppearance appearance)
        {
            _appearance = appearance;
            _currentStats = _defaultStats = stats;
        }

        public UnitStats GetStats()
        {
            return _currentStats;
        }

        public UnitAppearance GetAppearance()
        {
            return _appearance;
        }
        #endregion SetUp
        
        #region Health
        public void GetDamage(int value)
        {
            if (isDead || value <= 0)
                return;
            int old = _currentStats.healthPoints;
            if (_currentStats.healthPoints >= value)
                _currentStats.healthPoints -= value;
            else
                _currentStats.healthPoints = 0;
            onHPChanged?.Invoke(_currentStats.healthPoints - old);
        }

        public void GetHealed(int value)
        {
            if (isDead || value <= 0)
                return;
            int old = _currentStats.healthPoints;
            if (_defaultStats.healthPoints >= value + _currentStats.healthPoints)
                _currentStats.healthPoints += value;
            else
                _currentStats.healthPoints = _defaultStats.healthPoints;
            onHPChanged?.Invoke(_currentStats.healthPoints - old);
        }

        public void GetRevived()
        {
            if (!isDead)
                return;
            
            _currentStats.healthPoints = 1;
            onHPChanged?.Invoke(1);
        }
        #endregion Health
        
        #region Position
        public void SetPositionSilent(Vector3 pos)
        {
            _position = pos;
        }

        public void SetPosition(Vector3 pos)
        {
            _position = pos;
            onPositionChanged?.Invoke(pos);
        }

        public Vector3 GetPosition()
        {
            return _position;
        }
        #endregion Position
    }
}
