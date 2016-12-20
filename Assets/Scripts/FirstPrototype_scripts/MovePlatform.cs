using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{

	private HingeJoint2D joint;
	private JointMotor2D motor;
	private float motorSpeed;

	// Use this for initialization
	void Start ()
	{
		joint = GetComponent<HingeJoint2D> () as HingeJoint2D;
		motor = joint.motor;
		motorSpeed = motor.motorSpeed;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (joint.limitState == JointLimitState2D.UpperLimit) {
			motor.motorSpeed = motorSpeed;
			joint.motor = motor;
		} else if (joint.limitState == JointLimitState2D.LowerLimit) {
			motor.motorSpeed = -motorSpeed;
			joint.motor = motor;
		}
	}
}
