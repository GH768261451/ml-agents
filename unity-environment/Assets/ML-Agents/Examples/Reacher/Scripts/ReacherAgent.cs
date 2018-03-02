﻿using UnityEngine;

public class ReacherAgent : Agent {

    public GameObject pendulumA;
    public GameObject pendulumB;
    public GameObject hand;
    public GameObject goal;
    float goalDegree;
    Rigidbody rbA;
    Rigidbody rbB;
    float goalSpeed;

    /// <summary>
    /// Collect the rigidbodies of the reacher in order to resue them for 
    /// observations and actions.
    /// </summary>
    public override void InitializeAgent()
    {
        rbA = pendulumA.GetComponent<Rigidbody>();
        rbB = pendulumB.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// We collect the normalized rotations, angularal velocities, and velocities of both
    /// limbs of the reacher as well as the relative position of the target and hand.
    /// </summary>
	public override void CollectObservations()
	{
        AddVectorObs(pendulumA.transform.rotation.eulerAngles.x / 180.0f - 1.0f);
        AddVectorObs(pendulumA.transform.rotation.eulerAngles.y / 180.0f - 1.0f);
        AddVectorObs(pendulumA.transform.rotation.eulerAngles.z / 180.0f - 1.0f);
        AddVectorObs(rbA.angularVelocity.x);
        AddVectorObs(rbA.angularVelocity.y);
        AddVectorObs(rbA.angularVelocity.z);
        AddVectorObs(rbA.velocity.x);
        AddVectorObs(rbA.velocity.y);
        AddVectorObs(rbA.velocity.z);

        AddVectorObs(pendulumB.transform.rotation.eulerAngles.x / 180.0f - 1.0f);
        AddVectorObs(pendulumB.transform.rotation.eulerAngles.y / 180.0f - 1.0f);
        AddVectorObs(pendulumB.transform.rotation.eulerAngles.z / 180.0f - 1.0f);
        AddVectorObs(rbB.angularVelocity.x);
        AddVectorObs(rbB.angularVelocity.y);
        AddVectorObs(rbB.angularVelocity.z);
        AddVectorObs(rbB.velocity.x);
        AddVectorObs(rbB.velocity.y);
        AddVectorObs(rbB.velocity.z);

        Vector3 localGoalPosition = goal.transform.position - transform.position;
        AddVectorObs(localGoalPosition.x);
        AddVectorObs(localGoalPosition.y);
        AddVectorObs(localGoalPosition.z);

        Vector3 localHandPosition = hand.transform.position - transform.position;
        AddVectorObs(localHandPosition.x);
        AddVectorObs(localHandPosition.y);
        AddVectorObs(localHandPosition.z);
	}

    /// <summary>
    /// The agent's four actions correspond to torques on each of the two joints.
    /// </summary>
	public override void AgentAction(float[] act)
	{
        goalDegree += goalSpeed;
        UpdateGoalPosition();

        float torque_x = Mathf.Clamp(act[0], -1, 1) * 100f;
        float torque_z = Mathf.Clamp(act[1], -1, 1) * 100f;
        rbA.AddTorque(new Vector3(torque_x, 0f, torque_z));

        torque_x = Mathf.Clamp(act[2], -1, 1) * 100f;
        torque_z = Mathf.Clamp(act[3], -1, 1) * 100f;
        rbB.AddTorque(new Vector3(torque_x, 0f, torque_z));
	}

    /// <summary>
    /// Used to move the position of the target goal around the agent.
    /// </summary>
    void UpdateGoalPosition() {
        float radians = (goalDegree * Mathf.PI) / 180f;
        float goalX = 8f * Mathf.Cos(radians);
        float goalY = 8f * Mathf.Sin(radians);

        goal.transform.position = new Vector3(goalY, -1f, goalX) + transform.position;
    }

    /// <summary>
    /// Resets the position and velocity of the agent and the goal.
    /// </summary>
    public override void AgentReset()
    {
        pendulumA.transform.position = new Vector3(0f, -4f, 0f) + transform.position;
        pendulumA.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
        rbA.velocity = new Vector3(0f, 0f, 0f);
        rbA.angularVelocity = new Vector3(0f, 0f, 0f);

        pendulumB.transform.position = new Vector3(0f, -10f, 0f) + transform.position;
        pendulumB.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
        rbB.velocity = new Vector3(0f, 0f, 0f);
        rbB.angularVelocity = new Vector3(0f, 0f, 0f);


        goalDegree = Random.Range(0, 360);
        UpdateGoalPosition();

        ReacherAcademy academy = GameObject.Find("Academy").GetComponent<ReacherAcademy>();
        float goalSize = academy.goalSize;
        goalSpeed = academy.goalSpeed;

        goal.transform.localScale = new Vector3(goalSize, goalSize, goalSize);
    }

}