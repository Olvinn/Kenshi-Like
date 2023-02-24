using System.Collections.Generic;
using NUnit.Framework;
using Units.MVC.Model;
using Units.Structures;
using UnityEngine;

namespace Units.Tests.EditorTests
{
    public class UnitModelTest
    {
        [Test]
        public void UnitStatsTests()
        {
            UnitStats stats = TestUtils.GetTestStats();
            
            //just make sure it all of value types
            UnitStats copy = stats;
            copy.healthPoints -= 10;
            copy.attackPower *= 2f;
            copy.speed = 4;
            
            Assert.AreEqual(90, copy.healthPoints);
            Assert.AreEqual(100, stats.healthPoints);
            Assert.AreEqual(20, copy.attackPower);
            Assert.AreEqual(10, stats.attackPower);
            Assert.AreEqual(4, copy.speed);
            Assert.AreEqual(5, stats.speed);
        }
        
        [Test]
        public void UnitAppearanceTests()
        {
            UnitAppearance appearance = TestUtils.GetTestAppearance();
            
            //just make sure it all of value types
            UnitAppearance copy = appearance;
            copy.skinColor = Color.blue;
            copy.baseColor = Color.blue;
            copy.secondaryColor = Color.blue;
            copy.accentColor = Color.blue;
            
            Assert.AreEqual(Color.blue, copy.skinColor);
            Assert.AreEqual(Color.blue, copy.baseColor);
            Assert.AreEqual(Color.blue, copy.secondaryColor);
            Assert.AreEqual(Color.blue, copy.accentColor);
            Assert.AreEqual(Color.white, appearance.skinColor);
            Assert.AreEqual(Color.red, appearance.baseColor);
            Assert.AreEqual(Color.black, appearance.secondaryColor);
            Assert.AreEqual(Color.cyan, appearance.accentColor);
        }
        
        [Test]
        public void UnitModelSimpleTests()
        {
            UnitStats stats = TestUtils.GetTestStats();
            UnitAppearance appearance = TestUtils.GetTestAppearance();
            UnitModel model = new UnitModel(stats, appearance);
            
            Assert.AreEqual(stats, model.GetStats());
            Assert.AreEqual(appearance, model.GetAppearance());
            Assert.AreEqual(false,model.isDead);
        }
        
        [Test]
        public void UnitModelDamageAndHealTests()
        {
            UnitStats stats = TestUtils.GetTestStats();
            UnitAppearance appearance = TestUtils.GetTestAppearance();
            UnitModel model = new UnitModel(stats, appearance);
            
            Assert.AreEqual(false,model.isDead);
            model.GetDamage(200);
            Assert.AreEqual(true,model.isDead);
            model.GetDamage(-200);
            Assert.AreEqual(true,model.isDead);
            model.GetHealed(200);
            Assert.AreEqual(true,model.isDead);
            model.GetRevived();
            Assert.AreEqual(false,model.isDead);
            Assert.AreEqual(1,model.GetStats().healthPoints);
            model.GetHealed(200);
            Assert.AreEqual(stats,model.GetStats());
            model.GetDamage(50);
            Assert.AreEqual(50,model.GetStats().healthPoints);
            model.GetDamage(-50);
            Assert.AreEqual(50,model.GetStats().healthPoints);
            model.GetHealed(-200);
            Assert.AreEqual(50,model.GetStats().healthPoints);
        }
        
        [Test]
        public void UnitModelEventsTests()
        {
            UnitStats stats = TestUtils.GetTestStats();
            UnitAppearance appearance = TestUtils.GetTestAppearance();
            UnitModel model = new UnitModel(stats, appearance);

            List<int> history = new List<int>();
            model.onHPChanged += (x) =>
            {
                history.Add(x);
            };

            model.UpdatePosition(Vector3.one);
            model.GetDamage(200);
            model.GetDamage(-200);
            model.GetHealed(200);
            model.GetRevived();
            model.GetHealed(200);
            model.GetDamage(50);
            model.GetDamage(-50);
            model.GetHealed(-200);
            
            Assert.AreEqual(4, history.Count);
            Assert.AreEqual(-100, history[0]);
            Assert.AreEqual(1, history[1]);
            Assert.AreEqual(99, history[2]);
            Assert.AreEqual(-50, history[3]);
        }
    }
}
