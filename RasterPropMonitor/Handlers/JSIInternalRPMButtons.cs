﻿/*****************************************************************************
 * RasterPropMonitor
 * =================
 * Plugin for Kerbal Space Program
 *
 *  by Mihara (Eugene Medvedev), MOARdV, and other contributors
 * 
 * RasterPropMonitor is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, revision
 * date 29 June 2007, or (at your option) any later version.
 * 
 * RasterPropMonitor is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with RasterPropMonitor.  If not, see <http://www.gnu.org/licenses/>.
 ****************************************************************************/

using KSP.UI;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JSI
{
    /// <summary>
    /// Provides a built-in plugin to execute tasks that can be done in the
    /// core RPM without plugin assistance.
    /// </summary>
    public class JSIInternalRPMButtons : IJSIModule
    {
        public JSIInternalRPMButtons(Vessel myVessel)
        {
            vessel = myVessel;
        }

        /// <summary>
        /// Turns on the flowState for all resources on the ship.
        /// </summary>
        /// <param name="state"></param>
        public void ButtonActivateReserves(bool state)
        {
            if (vessel == null)
            {
                return;
            }

            foreach (Part thatPart in vessel.parts)
            {
                foreach (PartResource resource in thatPart.Resources)
                {
                    resource.flowState = true;
                }
            }
        }

        /// <summary>
        /// Indicates whether any onboard resources have been flagged as
        /// reserves by switching them to "not usable".
        /// </summary>
        /// <returns></returns>
        public bool ButtonActivateReservesState()
        {
            if (vessel != null)
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                return comp.resourcesLocked;
            }
            return false;
        }

        /// <summary>
        /// Clear all maneuver nodes
        /// </summary>
        /// <param name="state"></param>
        // Analysis disable once UnusedParameter
        public void ButtonClearNodes(bool state)
        {
            if (vessel != null)
            {
                JUtil.RemoveAllNodes(vessel.patchedConicSolver);
            }
        }

        /// <summary>
        /// Indicates whether there are maneuver nodes to clear.
        /// </summary>
        /// <returns></returns>
        public bool ButtonClearNodesState()
        {
            if (vessel == null || vessel.patchedConicSolver == null)
            {
                // patchedConicSolver can be null in early career mode.
                return false;
            }

            return (vessel.patchedConicSolver.maneuverNodes.Count > 0);
        }

        /// <summary>
        /// Clear the current target.
        /// </summary>
        /// <param name="state"></param>
        public void ButtonClearTarget(bool state)
        {
            FlightGlobals.fetch.SetVesselTarget((ITargetable)null);
        }

        /// <summary>
        /// Returns whether there are any targets to clear.
        /// </summary>
        /// <returns></returns>
        public bool ButtonClearTargetState()
        {
            return (FlightGlobals.fetch.VesselTarget != null);
        }

        /// <summary>
        /// Toggles engines on the current stage (on/off)
        /// </summary>
        /// <param name="state">"true" for on, "false" for off</param>
        public void ButtonEnableEngines(bool state)
        {
            if (vessel != null)
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                comp.SetEnableEngines(state);
            }
        }

        /// <summary>
        /// Indicates whether at least one engine is enabled.
        /// </summary>
        /// <returns></returns>
        public bool ButtonEnableEnginesState()
        {
            if (vessel != null)
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                return comp.anyEnginesEnabled;
            }
            return false;
        }

        /// <summary>
        /// Allows enabling/disabling electric generators (and fuel cells)
        /// </summary>
        /// <param name="state"></param>
        public void ButtonEnableElectricGenerator(bool state)
        {
            if (vessel != null)
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                comp.SetEnableGenerators(state);
            }
        }

        /// <summary>
        /// Returns whether any generators or fuel cells are active.
        /// </summary>
        /// <returns></returns>
        public bool ButtonEnableElectricGeneratorState()
        {
            if (vessel != null)
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                return comp.generatorsActive;
            }

            return false;
        }

        /// <summary>
        /// Toggle Precision Input mode
        /// </summary>
        /// <param name="state"></param>
        public void ButtonPrecisionMode(bool state)
        {
            if (vessel != null)
            {
                FlightInputHandler.fetch.precisionMode = state;

                // Update the UI.
                // MOARdV: In 1.1, this only affects the normal flight display,
                // not the docking mode display.
                var gauges = UnityEngine.Object.FindObjectOfType<KSP.UI.Screens.Flight.LinearControlGauges>();
                if (gauges != null)
                {
                    //JUtil.LogMessage(this, "{0} input gauge images", gauges.inputGaugeImages.Count);
                    for (int i = 0; i < gauges.inputGaugeImages.Count; ++i)
                    {
                        gauges.inputGaugeImages[i].color = (state) ? XKCDColors.BrightCyan : XKCDColors.Orange;
                    }
                }
            }
        }

        /// <summary>
        /// Returns 'true' if the inputs are in precision mode.
        /// </summary>
        /// <returns></returns>
        public bool ButtonPrecisionModeState()
        {
            return FlightInputHandler.fetch.precisionMode;
        }

        /// <summary>
        /// returns a boolean indicating whether the specified SAS mode is active
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private bool ButtonSASModeState(VesselAutopilot.AutopilotMode mode)
        {
            return vessel != null && vessel.Autopilot.GetActualMode() == mode;
        }

        /// <summary>
        /// Common function for setting the autopilot mode
        /// </summary>
        /// <param name="mode"></param>
        private void ButtonSASModeClick(VesselAutopilot.AutopilotMode mode)
        {
            if (vessel != null)
            {
                vessel.Autopilot.SetActualMode(mode);
            }
        }

        /// <summary>
        /// Sets SAS to stability assist mode
        /// </summary>
        /// <param name="ignored">Unused</param>
        // Analysis disable once UnusedParameter
        public void ButtonSASModeStabilityAssist(bool ignored)
        {
            ButtonSASModeClick(VesselAutopilot.AutopilotMode.StabilityAssist);
        }

        /// <summary>
        /// Used to check SAS mode.
        /// </summary>
        /// <returns>true if SAS is currently set for Stability Assist</returns>
        public bool ButtonSASModeStabilityAssistState()
        {
            return ButtonSASModeState(VesselAutopilot.AutopilotMode.StabilityAssist);
        }

        /// <summary>
        /// Sets SAS to prograde mode
        /// </summary>
        /// <param name="ignored">Unused</param>
        // Analysis disable once UnusedParameter
        public void ButtonSASModePrograde(bool ignored)
        {
            ButtonSASModeClick(VesselAutopilot.AutopilotMode.Prograde);
        }

        /// <summary>
        /// Used to check SAS mode.
        /// </summary>
        /// <returns>true if SAS is currently set for Prograde</returns>
        public bool ButtonSASModeProgradeState()
        {
            return ButtonSASModeState(VesselAutopilot.AutopilotMode.Prograde);
        }

        /// <summary>
        /// Sets SAS to retrograde mode
        /// </summary>
        /// <param name="ignored">Unused</param>
        // Analysis disable once UnusedParameter
        public void ButtonSASModeRetrograde(bool ignored)
        {
            ButtonSASModeClick(VesselAutopilot.AutopilotMode.Retrograde);
        }

        /// <summary>
        /// Used to check SAS mode.
        /// </summary>
        /// <returns>true if SAS is currently set for Retrograde</returns>
        public bool ButtonSASModeRetrogradeState()
        {
            return ButtonSASModeState(VesselAutopilot.AutopilotMode.Retrograde);
        }

        /// <summary>
        /// Sets SAS to normal mode
        /// </summary>
        /// <param name="ignored">Unused</param>
        // Analysis disable once UnusedParameter
        public void ButtonSASModeNormal(bool ignored)
        {
            ButtonSASModeClick(VesselAutopilot.AutopilotMode.Normal);
        }

        /// <summary>
        /// Used to check SAS mode.
        /// </summary>
        /// <returns>true if SAS is currently set for Normal</returns>
        public bool ButtonSASModeNormalState()
        {
            return ButtonSASModeState(VesselAutopilot.AutopilotMode.Normal);
        }

        /// <summary>
        /// Sets SAS to anti normal mode
        /// </summary>
        /// <param name="ignored">Unused</param>
        // Analysis disable once UnusedParameter
        public void ButtonSASModeAntiNormal(bool ignored)
        {
            ButtonSASModeClick(VesselAutopilot.AutopilotMode.Antinormal);
        }

        /// <summary>
        /// Used to check SAS mode.
        /// </summary>
        /// <returns>true if SAS is currently set for Antinormal</returns>
        public bool ButtonSASModeAntiNormalState()
        {
            return ButtonSASModeState(VesselAutopilot.AutopilotMode.Antinormal);
        }

        /// <summary>
        /// Sets SAS to radial in mode
        /// </summary>
        /// <param name="ignored">Unused</param>
        // Analysis disable once UnusedParameter
        public void ButtonSASModeRadialIn(bool ignored)
        {
            ButtonSASModeClick(VesselAutopilot.AutopilotMode.RadialIn);
        }

        /// <summary>
        /// Used to check SAS mode.
        /// </summary>
        /// <returns>true if SAS is currently set for RadialIn</returns>
        public bool ButtonSASModeRadialInState()
        {
            return ButtonSASModeState(VesselAutopilot.AutopilotMode.RadialIn);
        }

        /// <summary>
        /// Sets SAS to radial out mode
        /// </summary>
        /// <param name="ignored">Unused</param>
        // Analysis disable once UnusedParameter
        public void ButtonSASModeRadialOut(bool ignored)
        {
            ButtonSASModeClick(VesselAutopilot.AutopilotMode.RadialOut);
        }

        /// <summary>
        /// Used to check SAS mode.
        /// </summary>
        /// <returns>true if SAS is currently set for RadialOut</returns>
        public bool ButtonSASModeRadialOutState()
        {
            return ButtonSASModeState(VesselAutopilot.AutopilotMode.RadialOut);
        }

        /// <summary>
        /// Sets SAS to target mode
        /// </summary>
        /// <param name="ignored">Unused</param>
        // Analysis disable once UnusedParameter
        public void ButtonSASModeTarget(bool ignored)
        {
            ButtonSASModeClick(VesselAutopilot.AutopilotMode.Target);
        }

        /// <summary>
        /// Used to check SAS mode.
        /// </summary>
        /// <returns>true if SAS is currently set for Target</returns>
        public bool ButtonSASModeTargetState()
        {
            return ButtonSASModeState(VesselAutopilot.AutopilotMode.Target);
        }

        /// <summary>
        /// Sets SAS to anti target mode
        /// </summary>
        /// <param name="ignored">Unused</param>
        // Analysis disable once UnusedParameter
        public void ButtonSASModeAntiTarget(bool ignored)
        {
            ButtonSASModeClick(VesselAutopilot.AutopilotMode.AntiTarget);
        }

        /// <summary>
        /// Used to check SAS mode.
        /// </summary>
        /// <returns>true if SAS is currently set for AntiTarget</returns>
        public bool ButtonSASModeAntiTargetState()
        {
            return ButtonSASModeState(VesselAutopilot.AutopilotMode.AntiTarget);
        }

        /// <summary>
        /// Sets SAS to maneuver mode mode
        /// </summary>
        /// <param name="ignored">Unused</param>
        // Analysis disable once UnusedParameter
        public void ButtonSASModeManeuver(bool ignored)
        {
            ButtonSASModeClick(VesselAutopilot.AutopilotMode.Maneuver);
        }

        /// <summary>
        /// Used to check SAS mode.
        /// </summary>
        /// <returns>true if SAS is currently set for Maneuver</returns>
        public bool ButtonSASModeManeuverState()
        {
            return ButtonSASModeState(VesselAutopilot.AutopilotMode.Maneuver);
        }

        /**
         * Cycle speed modes (between orbital/surface/target)
         */
        public void ButtonSpeedMode(bool ignored)
        {
            FlightGlobals.CycleSpeedModes();
        }

        /**
         * Returns true (really, nothing makes sense for the return value).
         */
        public bool ButtonSpeedModeState()
        {
            return true;
        }

        /**
         * Toggles the staging lock.
         *
         * WARNING: We are using the same string as KSP, so that our lock will
         * interact with the game's lock (alt-L); if an update to KSP changes
         * the name they use, we will have to be updated.
         */
        public void ButtonStageLock(bool state)
        {
            if (state)
            {
                InputLockManager.SetControlLock(ControlTypes.STAGING, "manualStageLock");
            }
            else
            {
                InputLockManager.RemoveControlLock("manualStageLock");
            }
        }

        /**
         * Returns whether staging is locked (disabled).
         */
        public bool ButtonStageLockState()
        {
            return InputLockManager.IsLocked(ControlTypes.STAGING);
        }

        /// <summary>
        /// Cuts throttle (set it to 0) when state is true.
        /// </summary>
        /// <param name="state"></param>
        public void ButtonCutThrottle(bool state)
        {
            if (state && vessel != null)
            {
                float throttle = vessel.ctrlState.mainThrottle;
                try
                {
                    FlightInputHandler.state.mainThrottle = 0.0f;
                }
                catch (Exception)
                {
                    FlightInputHandler.state.mainThrottle = throttle;
                }
            }
        }

        /// <summary>
        /// Returns true when the throttle is at or near 0.
        /// </summary>
        /// <returns></returns>
        public bool ButtonCutThrottleState()
        {
            return ((vessel != null) && vessel.ctrlState.mainThrottle < 0.01f);
        }

        /// <summary>
        /// Sets the throttle to maximum (1.0) when state is true.
        /// </summary>
        /// <param name="state"></param>
        public void ButtonFullThrottle(bool state)
        {
            if (state && vessel != null)
            {
                float throttle = vessel.ctrlState.mainThrottle;
                try
                {
                    FlightInputHandler.state.mainThrottle = 1.0f;
                }
                catch (Exception)
                {
                    FlightInputHandler.state.mainThrottle = throttle;
                }
            }
        }

        /// <summary>
        /// Set the throttle to the desired setting in the range [0-100]
        /// </summary>
        /// <param name="setting"></param>
        public void SetThrottle(double setting)
        {
            if (vessel != null)
            {
                float newThrottle = Mathf.Clamp01((float)setting / 100.0f);
                float throttle = vessel.ctrlState.mainThrottle;

                try
                {
                    // Why was this in a try with a catch that does the same thing?
                    FlightInputHandler.state.mainThrottle = newThrottle;
                }
                catch (Exception)
                {
                    FlightInputHandler.state.mainThrottle = throttle;
                }
            }
        }

        /// <summary>
        /// Returns when the throttle is at or near maximum.
        /// </summary>
        /// <returns></returns>
        public bool ButtonFullThrottleState()
        {
            return ((vessel != null) && vessel.ctrlState.mainThrottle > 0.99f);
        }

        /// <summary>
        /// Recover the vessel.  Only recovers when the vessel is recoverable,
        /// regardless of the parameter.
        /// </summary>
        /// <param name="ignored">Ignored</param>
        public void RecoverVessel(bool ignored)
        {
            if (vessel != null && vessel.IsRecoverable)
            {
                JSI.Core.JSIVesselRecovery.Recover(vessel);
            }
        }

        /// <summary>
        /// Returns true when the "Recover Vessel" feature is true.
        /// </summary>
        /// <returns></returns>
        public bool CanRecoverVessel()
        {
            return (vessel != null) ? vessel.IsRecoverable : false;
        }

        /// <summary>
        /// Undock the current reference part, or the inferred first dock on
        /// the current vessel.
        /// 
        /// The state of the dock appears to be queriable only by reading a
        /// string.  The possible values of that string (that I've discovered)
        /// are:
        /// 
        /// "Disabled", for shielded docking ports that are closed.
        /// "Docked (dockee)", for docks that were docked to (recipient dock).
        /// "Docked (docker)", for docks that initiated the docking.
        /// "PreAttached", for docks that were attached to something in the VAB
        /// "Ready", for docks that are ready.
        /// </summary>
        /// <param name="state">New state - must be 'false' to trigger the undock event</param>
        public void DockUndock(bool state)
        {
            if (vessel == null || state == true)
            {
                return;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            if (comp.mainDockingNodeState == RPMVesselComputer.DockingNodeState.DOCKED)
            {
                comp.mainDockingNode.Undock();
            }
        }

        /// <summary>
        /// Detach a docking node that was attached in the VAB.
        /// </summary>
        /// <param name="state">New state - must be 'false' to trigger</param>
        public void DockDetach(bool state)
        {
            if (vessel == null || state == true)
            {
                return;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            if (comp.mainDockingNodeState == RPMVesselComputer.DockingNodeState.PREATTACHED)
            {
                comp.mainDockingNode.Decouple();
            }
        }

        /// <summary>
        /// Is the current reference dock pre-attached ("docked" in the VAB)?
        /// </summary>
        /// <returns></returns>
        public bool DockAttached()
        {
            if (vessel == null)
            {
                return false;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            return (comp.mainDockingNodeState == RPMVesselComputer.DockingNodeState.PREATTACHED);
        }

        /// <summary>
        /// Is the current reference dock docked to something?
        /// </summary>
        /// <returns></returns>
        public bool DockDocked()
        {
            if (vessel == null)
            {
                return false;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            return (comp.mainDockingNodeState == RPMVesselComputer.DockingNodeState.DOCKED);
        }

        /// <summary>
        /// Is the current reference dock ready?
        /// </summary>
        /// <returns></returns>
        public bool DockReady()
        {
            if (vessel == null)
            {
                return false;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            return (comp.mainDockingNodeState == RPMVesselComputer.DockingNodeState.READY);
        }

        /// <summary>
        /// Returns a value representing the current state of the landing gear.
        /// </summary>
        /// <returns></returns>
        public double LandingGearState()
        {
            if (vessel == null)
            {
                return -1.0;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            return (double)comp.gearState;
        }

        /// <summary>
        /// Returns a value representing the current position of landing gear
        /// where 0 is retracted, 1 is extended.
        /// </summary>
        /// <returns></returns>
        public double LandingGearPosition()
        {
            if (vessel == null)
            {
                return 0.0;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            return (double)comp.gearPosition;
        }

        /// <summary>
        /// Toggles thrust reversers
        /// </summary>
        /// <param name="state"></param>
        public void SetThrustReverser(bool state)
        {
            if (vessel != null)
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                for (int i = 0; i < comp.availableThrustReverser.Count; ++i)
                {
                    ModuleAnimateGeneric thrustReverser = comp.availableThrustReverser[i].thrustReverser;
                    if (thrustReverser != null)
                    {
                        if (state)
                        {
                            if (thrustReverser.Progress < 0.5f && thrustReverser.CanMove && thrustReverser.aniState != ModuleAnimateGeneric.animationStates.MOVING)
                            {
                                thrustReverser.Toggle();
                            }
                        }
                        else
                        {
                            if (thrustReverser.Progress > 0.5f && thrustReverser.CanMove && thrustReverser.aniState != ModuleAnimateGeneric.animationStates.MOVING)
                            {
                                thrustReverser.Toggle();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if at least one thrust reverser is enabled.
        /// </summary>
        /// <returns></returns>
        public bool GetThrustReverserEnabled()
        {
            if (vessel == null)
            {
                return false;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            return comp.anyThrustReversersDeployed;
        }

        /// <summary>
        /// Returns the wheel brakes tweakable (averaged across wheels)
        /// </summary>
        /// <returns></returns>
        public double GetWheelBrakes()
        {
            if (vessel == null)
            {
                return 0.0;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            return (double)comp.wheelBrakeSetting;
        }

        /// <summary>
        /// Adjust the wheel brake tweakable
        /// </summary>
        /// <param name="setting"></param>
        public void SetWheelBrakes(double setting)
        {
            if (vessel != null)
            {
                float newsetting = Mathf.Clamp((float)setting, 0.0f, 200.0f);
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                for (int i = 0; i < comp.availableWheelBrakes.Count; ++i)
                {
                    comp.availableWheelBrakes[i].brakeTweakable = newsetting;
                }
            }
        }

        /// <summary>
        /// Returns true if any wheels are damaged
        /// </summary>
        /// <returns></returns>
        public bool GetWheelsDamaged()
        {
            if (vessel == null)
            {
                return false;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            return comp.wheelsDamaged;
        }

        /// <summary>
        /// Returns true if any wheels are repairable
        /// </summary>
        /// <returns></returns>
        public bool GetWheelsRepairable()
        {
            if (vessel == null)
            {
                return false;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            return comp.wheelsRepairable;
        }

        /// <summary>
        /// Returns the stress of the most-stressed wheel.
        /// </summary>
        /// <returns></returns>
        public double GetWheelStress()
        {
            if (vessel == null)
            {
                return 0.0;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            return comp.wheelStress;
        }

        /// <summary>
        /// Locks / unlocks gimbals on the currently-active stage.
        /// </summary>
        /// <param name="state"></param>
        public void GimbalLock(bool state)
        {
            if (vessel == null)
            {
                return;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            for (int i = 0; i < comp.availableGimbals.Count; ++i)
            {
                comp.availableGimbals[i].gimbalLock = state;
            }
        }

        /// <summary>
        /// Returns true if at least one gimbal on the active stage is locked.
        /// </summary>
        /// <returns></returns>
        public bool GimbalLockState()
        {
            if (vessel == null)
            {
                return false; // early
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            return comp.gimbalsLocked;
        }

        /// <summary>
        /// Toggle the state of any radar units installed on the craft.
        /// </summary>
        /// <param name="enabled"></param>
        public void RadarEnable(bool enabled)
        {
            if (vessel == null)
            {
                return;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            for (int i = 0; i < comp.availableRadars.Count; ++i)
            {
                comp.availableRadars[i].radarEnabled = enabled;
            }
        }

        /// <summary>
        /// Returns true if at least one radar is active.
        /// </summary>
        /// <returns></returns>
        public bool RadarEnableState()
        {
            if (vessel == null)
            {
                return false;
            }

            RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
            return comp.radarActive;
        }

        /// <summary>
        /// Returns true if at least one solar panel may be deployed.
        /// </summary>
        /// <returns></returns>
        public bool SolarPanelsDeployable()
        {
            if (vessel != null)
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                return comp.solarPanelsDeployable;
            }
            return false;
        }

        /// <summary>
        /// Returns true if at least one solar panel may be retracted.
        /// </summary>
        /// <returns></returns>
        public bool SolarPanelsRetractable()
        {
            if (vessel != null)
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                return comp.solarPanelsRetractable;
            }
            return false;
        }

        /// <summary>
        /// Returns the animation state of the first retractable solar panel
        /// that is not broken, unless they're all broken.
        /// </summary>
        /// <returns></returns>
        public double SolarPanelsState()
        {
            if (vessel != null)
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                return (double)comp.solarPanelMovement;
            }
            return -1.0;
        }

        /// <summary>
        /// Toggles the state of deployable solar panels.
        /// </summary>
        /// <param name="state"></param>
        public void SetDeploySolarPanels(bool state)
        {
            if (vessel != null)
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                comp.SetDeploySolarPanels(state);
            }
        }

        /// <summary>
        /// Inverse of SolarPanelsDeployable for use with SetDeploySolarPanels
        /// </summary>
        /// <returns></returns>
        public bool GetDeploySolarPanels()
        {
            if (vessel != null)
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                return comp.solarPanelsState;
            }
            return true;
        }

        /// <summary>
        /// Sets multi-mode engines to run in primary mode (true) or secondary
        /// mode (false).
        /// </summary>
        /// <param name="newstate"></param>
        public void SetEnginesPrimaryMode(bool newstate)
        {
            try
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                for (int i = 0; i < comp.availableMultiModeEngines.Count; ++i)
                {
                    if (comp.availableMultiModeEngines[i].runningPrimary ^ newstate)
                    {
                        if (newstate)
                        {
                            comp.availableMultiModeEngines[i].SetPrimary(true);
                        }
                        else
                        {
                            comp.availableMultiModeEngines[i].SetSecondary(true);
                        }
                        // Revised implementation:
                        //comp.availableMultiModeEngines[i].ModeEvent();

                        // original implementation:
                        //var ev = comp.availableMultiModeEngines[i].Events["ModeEvent"];
                        //if (ev != null)
                        //{
                        //    ev.Invoke();
                        //}
                    }
                }

                // Toggling modes changes which engines are enabled and which
                // are disabled.  Force a reset here.
                comp.InvalidateModuleLists();
            }
            catch { }
        }

        /// <summary>
        /// Returns true if any engines are running in primary mode (for multi-mode
        /// engines).
        /// </summary>
        /// <returns></returns>
        public bool GetEnginesPrimaryMode()
        {
            if (vessel != null)
            {
                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                return comp.anyMmePrimary;
            }
            return true;
        }

        /// <summary>
        /// Sets the throttle imit for all engines.
        /// </summary>
        /// <param name="limit"></param>
        public void SetThrottleLimit(double limit)
        {
            if (vessel != null)
            {
                if (limit < 0.0)
                {
                    limit = 0.0;
                }
                else if (limit > 100.0)
                {
                    limit = 100.0;
                }

                RPMVesselComputer comp = RPMVesselComputer.Instance(vessel);
                for (int i = 0; i < comp.availableEngines.Count; ++i)
                {
                    comp.availableEngines[i].thrustPercentage = (float)limit;
                }
            }
        }

        /// <summary>
        /// Returns a single numeric value indicating what mode the autopilot is in.
        /// </summary>
        /// <returns></returns>
        public double GetSASMode()
        {
            if (vessel == null)
            {
                return 0.0; // StabilityAssist
            }
            double mode;
            switch (vessel.Autopilot.GetActualMode())
            {
                case VesselAutopilot.AutopilotMode.StabilityAssist:
                    mode = 0.0;
                    break;
                case VesselAutopilot.AutopilotMode.Prograde:
                    mode = 1.0;
                    break;
                case VesselAutopilot.AutopilotMode.Retrograde:
                    mode = 2.0;
                    break;
                case VesselAutopilot.AutopilotMode.Normal:
                    mode = 3.0;
                    break;
                case VesselAutopilot.AutopilotMode.Antinormal:
                    mode = 4.0;
                    break;
                case VesselAutopilot.AutopilotMode.RadialIn:
                    mode = 5.0;
                    break;
                case VesselAutopilot.AutopilotMode.RadialOut:
                    mode = 6.0;
                    break;
                case VesselAutopilot.AutopilotMode.Target:
                    mode = 7.0;
                    break;
                case VesselAutopilot.AutopilotMode.AntiTarget:
                    mode = 8.0;
                    break;
                case VesselAutopilot.AutopilotMode.Maneuver:
                    mode = 9.0;
                    break;
                default:
                    mode = 0.0;
                    break;
            }
            return mode;
        }

        public void SetSASMode(double mode)
        {
            int imode = (int)mode;
            VesselAutopilot.AutopilotMode autopilotMode;
            switch (imode)
            {
                case 0:
                    autopilotMode = VesselAutopilot.AutopilotMode.StabilityAssist;
                    break;
                case 1:
                    autopilotMode = VesselAutopilot.AutopilotMode.Prograde;
                    break;
                case 2:
                    autopilotMode = VesselAutopilot.AutopilotMode.Retrograde;
                    break;
                case 3:
                    autopilotMode = VesselAutopilot.AutopilotMode.Normal;
                    break;
                case 4:
                    autopilotMode = VesselAutopilot.AutopilotMode.Antinormal;
                    break;
                case 5:
                    autopilotMode = VesselAutopilot.AutopilotMode.RadialIn;
                    break;
                case 6:
                    autopilotMode = VesselAutopilot.AutopilotMode.RadialOut;
                    break;
                case 7:
                    autopilotMode = VesselAutopilot.AutopilotMode.Target;
                    break;
                case 8:
                    autopilotMode = VesselAutopilot.AutopilotMode.AntiTarget;
                    break;
                case 9:
                    autopilotMode = VesselAutopilot.AutopilotMode.Maneuver;
                    break;
                default:
                    JUtil.LogErrorMessage(this, "SetSASMode: attempt to set a SAS mode with the invalid value {0}", imode);
                    return;
            }

            if (vessel != null)
            {
                vessel.Autopilot.SetActualMode(autopilotMode);
            }
        }

        /**
         * @returns true if all trim settings are within 1% of neutral.
         */
        public bool TrimNeutralState()
        {
            if (vessel != null && vessel.ctrlState != null)
            {
                return Mathf.Abs(vessel.ctrlState.pitchTrim) < 0.01f && Mathf.Abs(vessel.ctrlState.rollTrim) < 0.01f && Mathf.Abs(vessel.ctrlState.yawTrim) < 0.01f;
            }
            else
            {
                return true;
            }
        }

        /**
         * Resets all trim parameters to neutral
         */
        public void SetAllTrimNeutral(bool state)
        {
            FlightInputHandler.state.ResetTrim();
        }

        /**
         * Resets pitch trim to neutral
         */
        public void SetPitchTrimNeutral(bool state)
        {
            FlightInputHandler.state.pitchTrim = 0.0f;
        }

        /**
         * Sets pitch trim to the desired percent (-100 to 100)
         */
        public void SetPitchTrim(double trimPercent)
        {
            FlightInputHandler.state.pitchTrim = (float)(trimPercent.Clamp(-100.0, 100.0)) / 100.0f;
        }

        /**
         * Resets roll trim to neutral
         */
        public void SetRollTrimNeutral(bool state)
        {
            FlightInputHandler.state.rollTrim = 0.0f;
        }

        /**
         * Sets roll trim to the desired percent (-100 to 100)
         */
        public void SetRollTrim(double trimPercent)
        {
            FlightInputHandler.state.rollTrim = (float)(trimPercent.Clamp(-100.0, 100.0)) / 100.0f;
        }

        /**
         * Resets yaw trim to neutral
         */
        public void SetYawTrimNeutral(bool state)
        {
            FlightInputHandler.state.yawTrim = 0.0f;
        }

        /**
         * Sets yaw trim to the desired percent (-100 to 100)
         */
        public void SetYawTrim(double trimPercent)
        {
            FlightInputHandler.state.yawTrim = (float)(trimPercent.Clamp(-100.0, 100.0)) / 100.0f;
        }
    }
}