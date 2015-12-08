using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScanAtStationAction : Endeavour {

	LabelHandle scanStation;

	public ScanAtStationAction(RobotController controller, List<Goal> goals, LabelHandle scanStation)
		: base(controller, goals, scanStation) {
		this.name = "scanAtStation";
		this.scanStation = scanStation;
		requiredComponents = new System.Type[] {typeof(HoverJet)};
	}

	public override bool canExecute() {
		RobotArms arms = controller.GetComponentInChildren<RobotArms>();
		return (arms != null) && (arms.hasTarget());
	}

	public override void execute() {
		base.execute();
		HoverJet jet = controller.GetComponentInChildren<HoverJet>();
		if(jet != null) {
			jet.setTarget(scanStation, true);
			jet.setAvailability(false);
		}
	}

	public override void stopExecution() {
		base.stopExecution();
		HoverJet jet = controller.GetComponentInChildren<HoverJet>();
		if(jet != null) {
			jet.setTarget(null, false);
			jet.setAvailability(true);
		}
	}


	public override bool isStale() {
		return scanStation == null;
	}

	public override void onMessage(RobotMessage message) {
		if(message.Type == RobotMessage.MessageType.ACTION) {
			if(message.Message.Equals("target reached")) {
				LaserProjector projector = scanStation.label.GetComponentInChildren<LaserProjector>();
				if(projector != null) {
					projector.setController(controller);
					projector.startScan();
				}
				//RobotArms arms = controller.GetComponentInChildren<RobotArms>();
				//arms.dropTarget();
			}
			else if(message.Message.Equals("target scanned")) {
				List<Goal> goals = new List<Goal>();
				goals.Add(new Goal(GoalEnum.Offense, 10f));
				RobotArms arms = controller.GetComponentInChildren<RobotArms>();
				Label target = arms.getTarget();
				if(target.GetComponent<Player>() != null) {
					controller.addEndeavour(new ElectrocuteAction(controller, goals, target));
				}
			}
		}
	}

	protected override float getCost() {
		HoverJet jet = controller.GetComponentInChildren<HoverJet>();
		if(jet != null) {
			return jet.calculatePathCost(scanStation.label);
		}
		return 0;
	}
}
