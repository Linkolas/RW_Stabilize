using HarmonyLib;
using HugsLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Stabilize
{
    class Base : ModBase {

    }


    public static class Patches
    {
        [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
        public static class FloatMenuMakerCarryAdder
        {
            [HarmonyPostfix]
            public static void AddHumanlikeOrdersPostfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts) {

                if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || pawn.WorkTagIsDisabled(WorkTags.Caring) || pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) || !pawn.workSettings.WorkIsActive(WorkTypeDefOf.Doctor)) {
                    return;
                }

                foreach (LocalTargetInfo localTargetInfo in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true)) {

                    Pawn target = (Pawn) localTargetInfo.Thing;

                    if(!pawn.CanReserveAndReach(target, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true)) {
                        // TODO: Add grayed out message
                        continue;
                    }

                    JobDef stabilizeJD = DefDatabase<JobDef>.GetNamed("StabilizeHere");
                    //JobDef stabilizeJD = DefDatabase<JobDef>.GetNamed("TendPatient");

                    Action action = () => {
                        Job job = new Job(stabilizeJD, target);
                        job.count = 1;

                        pawn.jobs.TryTakeOrderedJob(job);
                    };

                    string text = "StabilizePawn".Translate(target.LabelCap, target);
                    opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text, action, MenuOptionPriority.RescueOrCapture, null, target, 0f, null, null), pawn, target, "ReservedBy"));
                }

                /*
                    string str2;
                Action action;
                Pawn pawn1;

                foreach (LocalTargetInfo localTargetInfo1 in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true)) {

                    LocalTargetInfo localTargetInfo2 = localTargetInfo1;
                    Pawn thing2 = (Pawn)localTargetInfo2.Thing;
                    if (!thing2.Downed || !pawn.CanReserveAndReach(thing2, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true) || Building_AMCell.FindAMCellFor(thing2, pawn, true) == null) {
                        continue;
                    }
                    string str3 = "Job_CarryToAMCell".Translate(new object[] { localTargetInfo2.Thing.LabelCap });
                    JobDef carryToAMCell = DefDatabase<JobDef>.GetNamed("JobDriver_StabilizeHere");

                    Action action1 = () => {
                        Building_AMCell buildingCryptosleepCasket = Building_AMCell.FindAMCellFor(thing2, pawn, false) ?? Building_AMCell.FindAMCellFor(thing2, pawn, true);
                        if (buildingCryptosleepCasket == null) {
                            Messages.Message(string.Concat("CannotCarryToAMCell".Translate(), ": ", "NoAMCell".Translate()), thing2, MessageTypeDefOf.RejectInput);
                            return;
                        }
                        Job job = new Job(carryToAMCell, thing2, buildingCryptosleepCasket) {
                            count = 1
                        };
                        pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    };

                    str2 = str3;
                    action = action1;
                    pawn1 = thing2;

                    opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(str2, action, MenuOptionPriority.Default, null, pawn1, 0f, null, null), pawn, thing2, "ReservedBy"));
                }
                */

            }
        }
    }
}
