using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.UIElements;

public class HamsterController : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private Animator animator;
  
  [Header("Values")]
  [SerializeField] private float forwardSpeed;
  [SerializeField] private float rotSpeed;
  [SerializeField] private float jumpHeight;
  [SerializeField] private float walkingThreshold;
  
  [Header("States")]
  [SerializeField] private bool isWalking;
  [SerializeField] private bool isBall;
  [SerializeField] private bool isJumping;
  [SerializeField] private bool isGrounded;
  
  private float _verticalInput, _horizontalInput;
  
  private void Awake()
  {
    isWalking = false;
    isBall = false;
    isJumping = false;
  }

  private void FixedUpdate()
  {
    _verticalInput = Input.GetAxis("Vertical");
    _horizontalInput = Input.GetAxis("Horizontal");

    if (_verticalInput != 0) Move();
    else if (isWalking)
    {
      animator.SetBool("IsWalking", false);
      isWalking = false;
    }
    
    if (_horizontalInput != 0) Rotate();

    if (Input.GetKeyDown(KeyCode.Space)) Jump();
  }

  private void Move()
  {
    if (!isBall && !isJumping)
    {
      animator.SetBool("IsWalking", true);
      isWalking = true;
    }
    transform.Translate(Vector3.forward * _verticalInput * forwardSpeed * Time.deltaTime);
  }

  private void Rotate()
  {
    transform.Rotate(Vector3.up * _horizontalInput * rotSpeed * Time.deltaTime);
  }

  private void Jump()
  {
    if (!isBall)
    {
      animator.SetBool("IsJumping", true);
      // isJumping = true;
    }
    transform.Translate(Vector3.up * jumpHeight * Time.deltaTime);
  }
}
