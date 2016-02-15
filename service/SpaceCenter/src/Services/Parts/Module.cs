using System.Collections.Generic;
using System.Linq;
using KRPC.Service.Attributes;
using KRPC.Utils;

namespace KRPC.SpaceCenter.Services.Parts
{
    /// <summary>
    /// In KSP, each part has zero or more
    /// <a href="http://wiki.kerbalspaceprogram.com/wiki/CFG_File_Documentation#MODULES">PartModules</a>
    /// associated with it. Each one contains some of the functionality of the part.
    /// For example, an engine has a "ModuleEngines" PartModule that contains all the
    /// functionality of an engine.
    ///
    /// This class allows you to interact with KSPs PartModules, and any PartModules
    /// that have been added by other mods.
    /// </summary>
    [KRPCClass (Service = "SpaceCenter")]
    public sealed class Module : Equatable<Module>
    {
        readonly Part part;
        readonly PartModule module;

        internal Module (Part part, PartModule module)
        {
            this.part = part;
            this.module = module;
        }

        /// <summary>
        /// Check if the modules are equal.
        /// </summary>
        public override bool Equals (Module obj)
        {
            return part == obj.part && module == obj.module;
        }

        /// <summary>
        /// Hash the module.
        /// </summary>
        public override int GetHashCode ()
        {
            return part.GetHashCode () ^ module.GetHashCode ();
        }

        /// <summary>
        /// Name of the PartModule. For example, "ModuleEngines".
        /// </summary>
        [KRPCProperty]
        public string Name {
            get { return module.moduleName; }
        }

        /// <summary>
        /// The part that contains this module.
        /// </summary>
        [KRPCProperty]
        public Part Part {
            get { return part; }
        }

        IEnumerable<BaseField> AllFields {
            get { return module.Fields.Cast<BaseField> ().Where (f => f != null && (HighLogic.LoadedSceneIsEditor ? f.guiActiveEditor : f.guiActive)); }
        }

        IEnumerable<BaseEvent> AllEvents {
            get { return module.Events.Where (e => e != null && (HighLogic.LoadedSceneIsEditor ? e.guiActiveEditor : e.guiActive) && e.active); }
        }

        IEnumerable<BaseAction> AllActions {
            get { return module.Actions; }
        }

        /// <summary>
        /// The modules field names and their associated values, as a dictionary.
        /// These are the values visible in the right-click menu of the part.
        /// </summary>
        [KRPCProperty]
        public IDictionary<string,string> Fields {
            get {
                var result = new Dictionary<string,string> ();
                foreach (var field in AllFields)
                    result [field.guiName] = field.GetValue (module).ToString ();
                return result;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the module has a field with the given name.
        /// </summary>
        /// <param name="name">Name of the field.</param>
        [KRPCMethod]
        public bool HasField (string name)
        {
            return AllFields.Any (x => x.guiName == name);
        }

        /// <summary>
        /// Returns the value of a field.
        /// </summary>
        /// <param name="name">Name of the field.</param>
        [KRPCMethod]
        public string GetField (string name)
        {
            return AllFields.First (x => x.guiName == name).GetValue (module).ToString ();
        }

        /// <summary>
        /// A list of the names of all of the modules events. Events are the clickable buttons
        /// visible in the right-click menu of the part.
        /// </summary>
        [KRPCProperty]
        public IList<string> Events {
            get { return AllEvents.Select (x => x.guiName).ToList (); }
        }

        /// <summary>
        /// <c>true</c> if the module has an event with the given name.
        /// </summary>
        /// <param name="name"></param>
        [KRPCMethod]
        public bool HasEvent (string name)
        {
            return AllEvents.Any (x => x.guiName == name);
        }

        /// <summary>
        /// Trigger the named event. Equivalent to clicking the button in the right-click menu of the part.
        /// </summary>
        /// <param name="name"></param>
        [KRPCMethod]
        public void TriggerEvent (string name)
        {
            AllEvents.First (x => x.guiName == name).Invoke ();
        }

        /// <summary>
        /// A list of all the names of the modules actions. These are the parts actions that can be assigned
        /// to action groups in the in-game editor.
        /// </summary>
        [KRPCProperty]
        public IList<string> Actions {
            get { return AllActions.Select (a => a.guiName).ToList (); }
        }

        /// <summary>
        /// <c>true</c> if the part has an action with the given name.
        /// </summary>
        /// <param name="name"></param>
        [KRPCMethod]
        public bool HasAction (string name)
        {
            return AllActions.Any (x => x.guiName == name);
        }

        /// <summary>
        /// Set the value of an action with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [KRPCMethod]
        public void SetAction (string name, bool value = true)
        {
            var action = AllActions.First (a => a.guiName == name);
            action.Invoke (new KSPActionParam (action.actionGroup, (value ? KSPActionType.Activate : KSPActionType.Deactivate)));
        }
    }
}
