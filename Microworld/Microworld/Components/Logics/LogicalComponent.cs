using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    public abstract class LogicalComponent
    {
        public Component parent;

        /// <summary>
        /// Use this to initialize logics
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Use this to reset the state of the logics
        /// </summary>
        public virtual void Reset()
        {
        }

        /// <summary>
        /// Called upon simulation start
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Called after every reinitialization of circuit, be it 
        /// starting simulation or placing / removing component.
        /// Use it for once-per-circuit checks.
        /// </summary>
        public virtual void CircuitInitialized()
        {
        }

        /// <summary>
        /// Called every circuit recalculation when the simulation is running
        /// Use MicroWorld.Logics.CircuitManager.ScheduleReupdate to implement logics
        /// </summary>
        public virtual void CircuitUpdate()
        {
        }

        /// <summary>
        /// Called after all circuit reupdates have been executed.
        /// Use it for example to disconnect unnecesarry test grounds that would mess up voltage drop otherwise
        /// Circuit will be recalculated automatically afterwards
        /// </summary>
        public virtual void LastCircuitUpdate()
        {
        }

        /// <summary>
        /// Called before every game step
        /// </summary>
        public virtual void PreUpdate()
        {
        }

        /// <summary>
        /// Called every game update when the simulation is running
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        /// Called upon program closing
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
