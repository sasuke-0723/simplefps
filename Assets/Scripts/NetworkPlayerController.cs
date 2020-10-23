﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System.Security.Cryptography;

/// <summary>
/// プレイヤーの基本操作を処理するコンポーネント
/// </summary>
[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class NetworkPlayerController : MonoBehaviour
{
    /// <summary>FPS のカメラ</summary>
    [SerializeField] Camera m_mainCamera;
    /// <summary>照準となる UI オブジェクト</summary>
    [SerializeField] Image m_crosshair;
    /// <summary>照準に敵を捕らえていない時の色</summary>    
    [SerializeField] Color m_noTarget;
    /// <summary>照準に敵を捕らえている時の色</summary>
    [SerializeField] Color m_onTarget;
    /// <summary>射撃可能距離</summary>
    [SerializeField, Range(10, 200)] float m_shootRange = 100f;
    /// <summary>照準の Ray が当たる Layer</summary>
    [SerializeField] LayerMask m_shootingLayer;

    /// <summary>攻撃したらダメージを与えられる対象</summary>
    Damageable m_target;
    PhotonView m_photonView;
    private CharacterController m_control;
    private Transform m_cameraTransform;

    private Vector3 m_moveDirection;
    private Vector3 m_camRotation;

    public float speed = 6;
    public float jumpSpeed = 1.5f;
    public float gravity = 0.5f;

    /// <summary>下に向くことのできる最小角度</summary>
    [Range(-45, -15)]
    public int minAngle = -30;
    /// <summary>上に向くことのできる最大角度</summary>
    [Range(30, 80)]
    public int maxAngle = 45;
    /// <summary>マウスの感度</summary>
    [Range(50, 500)]
    public int sensitivity = 200;

    public GameObject bullet = null;
    public GameObject flag = null;
    private bool haveFlag = false;
    private Vector3 playerPos;

    private void Start()
    {
        m_control = GetComponent<CharacterController>();
        m_photonView = GetComponent<PhotonView>();

        playerPos = GetComponent<Transform>().position;

        // 自分が生成したプレイヤーのカメラだけを有効にする
        if (m_photonView.IsMine)
        {
            m_mainCamera.gameObject.SetActive(true);
            m_cameraTransform = m_mainCamera.transform;
        }
        else
        {
            m_mainCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // 自分に対してだけ入力を検出して行われる処理をする
        if (m_photonView.IsMine)
        {
            Aim();
            Shoot();
            Rotate();
            Move();
        }
    }

    /// <summary>
    /// Controls camera rotation oriented by mouse position
    /// </summary>
    private void Rotate()
    {
        transform.Rotate(Vector3.up * sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));

        m_camRotation.x -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        m_camRotation.x = Mathf.Clamp(m_camRotation.x, minAngle, maxAngle);

        m_cameraTransform.localEulerAngles = m_camRotation;
    }

    /// <summary>
    /// 照準を操作する
    /// 照準にダメージを与えられる対象がいる場合に照準の色を変え、その対象を m_target に保存する
    /// </summary>
    void Aim()
    {
        Ray ray = m_mainCamera.ScreenPointToRay(m_crosshair.rectTransform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_shootRange, m_shootingLayer))
        {
            m_target = hit.collider.GetComponent<Damageable>();

            if (m_target)
            {
                m_crosshair.color = m_onTarget;
            }
            else
            {
                m_crosshair.color = m_noTarget;
            }
        }
        else
        {
            m_target = null;
            m_crosshair.color = m_noTarget;
        }

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
            playerPos = transform.position;
            Quaternion direction = new Quaternion(transform.rotation.x ,transform.rotation.y - 180, transform.rotation.z, transform.rotation.w);
            Instantiate(bullet, playerPos, direction);
		}
    }

    /// <summary>
    /// Controls movement with CharacterController
    /// </summary>
    private void Move()
    {
        if (m_control.isGrounded)
        {
            m_moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            m_moveDirection = transform.TransformDirection(m_moveDirection);

            if (Input.GetButtonDown("Jump"))
            {
                m_moveDirection.y = jumpSpeed;
            }
        }

        m_moveDirection.y -= gravity * Time.deltaTime;
        m_control.Move(m_moveDirection * speed * Time.deltaTime);
    }

    /// <summary>
    /// 敵を撃つ
    /// </summary>
    private void Shoot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (m_target)
            {
                m_target.Damage(PhotonNetwork.LocalPlayer.ActorNumber, 10);
            }
        }
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Flag"))
		{
            haveFlag = true;
		}
	}
}