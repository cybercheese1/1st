using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game;
using Sandbox.Definitions;

using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.Components;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.ModAPI;
using VRage.Game.ModAPI;
using VRage.Game;
//using Sandbox.ModAPI.Ingame;
using VRageMath;
using VRage.Game.Components;
//using Ingame = Sandbox.ModAPI.Ingame;

namespace StopCombatProjector {
  [VRage.Game.Components.MyEntityComponentDescriptor(typeof (Sandbox.Common.ObjectBuilders.MyObjectBuilder_Projector), true)]

  public class StopCombatProjector: MyGameLogicComponent {

    private VRage.ObjectBuilders.MyObjectBuilder_EntityBase _objectBuilder;
    private DateTime lastUpdate = DateTime.MinValue;
    private IMyProjector projector;

    public override void Close() {

    }

    public override void Init(VRage.ObjectBuilders.MyObjectBuilder_EntityBase objectBuilder) {
      _objectBuilder = objectBuilder;
      projector = (Entity as IMyProjector);

      if (projector != null && projector.BlockDefinition.ToString().Contains("projector"))
        Entity.NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;

    }

    public override void MarkForClose() {}

    public override void UpdateAfterSimulation() {}

    public override void UpdateBeforeSimulation() {}

    public override void UpdateAfterSimulation100() {}

    public override void UpdateAfterSimulation10() {
      try {

        if (projector == null || !projector.Enabled || !projector.IsWorking || !projector.IsFunctional) return;
        var projectorgrid = projector.GetTopMostParent();
        VRage.Game.ModAPI.IMyCubeGrid grid = (projectorgrid as VRage.Game.ModAPI.IMyCubeGrid);
        if (projectorgrid.Physics.LinearVelocity.Length() > 1) {

          List < VRage.Game.ModAPI.IMySlimBlock > blocks = new List < VRage.Game.ModAPI.IMySlimBlock > ();
          grid.GetBlocks(blocks, b => b != null && b.FatBlock != null && (b.FatBlock as IMyProjector) != null && b.FatBlock.IsWorking && b.FatBlock.IsFunctional &&
            b.FatBlock.BlockDefinition.ToString().Contains("MyObjectBuilder_Projector"));

          foreach(VRage.Game.ModAPI.IMySlimBlock b in blocks) {
            if ((b.FatBlock as IMyProjector).Enabled)
              (b.FatBlock as IMyProjector).RequestEnable(false);
            var damage = grid.GridSizeEnum.Equals(MyCubeSize.Large) ? 15.5 f : 0.05 f;
            b.DecreaseMountLevel(damage, null, true);
            b.ApplyAccumulatedDamage();
            MyAPIGateway.Utilities.ShowMessage(grid.CustomName, "projector failed because of high velocity!");
          }
        }
      } catch (Exception e) {

      }

    }

    public override void UpdateBeforeSimulation10() {}

    public override void UpdateBeforeSimulation100() {

    }

    public override void UpdateOnceBeforeFrame() {}

    public override VRage.ObjectBuilders.MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false) {
      return _objectBuilder;
    }
  }
}