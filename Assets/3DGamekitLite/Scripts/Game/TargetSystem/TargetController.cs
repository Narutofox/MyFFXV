using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gamekit3D;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._3DGamekitLite.Scripts.Game.TargetSystem
{
    public class TargetController : MonoBehaviour
    {

        private Camera _camera; //Main Camera
        internal EnemyController _target; //Current Focused Enemy In List
        private Image _targetImage; //Image Of Crosshair
        private bool _lockedOn; //Keeps Track Of Lock On Status            
        private int _lockedEnemy; //Tracks Which Enemy In List Is Current Target       
        private IList<EnemyController> _nearByEnemies; //List of nearby enemies
       
        private void Start()
        {
            _camera = Camera.main;
            _targetImage = GetComponent<Image>();
            _nearByEnemies = new List<EnemyController>();
            

            _lockedOn = false;
            _lockedEnemy = 0;
        }

        private void Update()
        {
            SetTargetIfAvailable();

            if (_lockedOn)
            {
                _target = _nearByEnemies[_lockedEnemy];

                if (_target == null)
                {
                    SetTargetIfAvailable();
                }
                else if (_target.transform == null)
                {
                    RemoveEnemyToList(_target.GetInstanceID());
                    SetTargetIfAvailable();
                }
                else
                {
                    //Determine Crosshair Location Based On The Current Target 
                    gameObject.transform.position = _camera.WorldToScreenPoint(_target.transform.position);

                    //Rotate Crosshair
                    gameObject.transform.Rotate(new Vector3(0, 0, -1));
                }
                
            }
        }

        private void SetTargetIfAvailable()
        {
            if (_nearByEnemies != null &&_nearByEnemies.Count >= 1)
            {
                _lockedOn = true;
                _targetImage.enabled = true;

               
                //Press E and Q To Switch Targets
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (_lockedEnemy >= _nearByEnemies.Count - 1)
                    {
                        _lockedEnemy = 0;
                        _target = GetEnemyFromList(_lockedEnemy);
                    }
                    else
                    {
                        _lockedEnemy++;
                        _target = GetEnemyFromList(_lockedEnemy);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (_lockedEnemy <= 0)
                    {
                        _lockedEnemy = _nearByEnemies.Count - 1;
                        _target = GetEnemyFromList(_lockedEnemy);
                    }
                    else
                    {
                        _lockedEnemy--;
                        _target = GetEnemyFromList(_lockedEnemy);
                    }
                }
                else if (_target == null)
                {
                    //Lock On To First Enemy In List By Default
                    _lockedEnemy = 0;
                    _target = GetEnemyFromList(_lockedEnemy);
                }
            }

            //Turn Off Lock On When No More Enemies Are In The List 
            else if (_nearByEnemies == null || _nearByEnemies.Count == 0)
            {
                _lockedOn = false;
                _targetImage.enabled = false;
                _lockedEnemy = 0;
                _target = null;
            }
        }

        public void AddEnemyToList(EnemyController enemyController)
        {
            if (_nearByEnemies.All(x => x.GetInstanceID() != enemyController.GetInstanceID()))
            {
                _nearByEnemies.Add(enemyController);
            }           
        }

        public void RemoveEnemyToList(int id)
        {         
            if (_nearByEnemies.Any(x=>x != null && x.GetInstanceID() == id))
            {
                _nearByEnemies.Remove(_nearByEnemies.First(x => x.GetInstanceID() == id));
            }

            if (_nearByEnemies.Any(x => x == null))
            {
                _nearByEnemies.ToList().RemoveAll(x => x == null);
            }
           
        }

        private EnemyController GetEnemyFromList(int index)
        {
            try
            {
                return _nearByEnemies[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                _lockedEnemy = 0;
                return null;
            }
        }
    }
}