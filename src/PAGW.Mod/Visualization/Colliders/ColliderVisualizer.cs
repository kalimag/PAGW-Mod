using UnityEngine;

namespace Kalimag.Modding.Unity.Visualization.Colliders
{
	internal abstract class ColliderVisualizer
    {

        private bool _startCalled;

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value != _enabled)
                {
                    _enabled = value;

                    if (value)
                    {
                        if (!_startCalled)
                        {
                            _startCalled = true;
                            Start();
                        }
                        OnEnable();
                    }
                    else
                    {
                        OnDisable();
                    }
                }
            }
        }

        public bool Stale { get; set; }

        private ColliderVisualizationConfig _config;
        public ColliderVisualizationConfig Config
        {
            get => _config;
            set
            {
                _config = value;
                OnConfigChanged();
            }
        }

        public Collider Collider { get; }

        public abstract void LateUpdate();

        protected abstract void Start();

        protected abstract void OnEnable();

        protected abstract void OnDisable();

        protected abstract void OnConfigChanged();

        public abstract void Destroy();



        public ColliderVisualizer(Collider collider)
        {
            Collider = collider;
        }


        public override string ToString()
        {
            return $"{GetType().Name} Config={Config.SelectorLabel} Enabled={Enabled} Stale={Stale} _startCalled={_startCalled}";
        }
    }
}
