using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private bool m_IsCrouching;
        [SerializeField] private bool m_IsAiming;
        [SerializeField] private float m_ShotForce;
		[SerializeField] private float m_ShotDamage;
        [SerializeField] private float m_ZoomFOV;
        [SerializeField] private float m_ZoomAimSpeed;
		[SerializeField] private float m_ZoomSpeed;
        [SerializeField] private float m_recoilForce;
        [SerializeField] private float m_CrosshairScale;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_CrouchDeltaHeight;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
        [SerializeField] private AudioClip m_FireSound;
		[SerializeField] private GameObject m_BulletImpactPrefab;

        private Camera m_Camera;
        private bool m_Jump;
        private bool m_isGrounded;
        private float m_YRotation;
        private float m_StandHeight;
        private float m_Zoomout;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private Vector3 m_HitNormal;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        private Rigidbody m_Rigidbody;
        private Animator m_animator;
		private Coroutine m_fireRoutine;
		private Camera m_scopeCamera;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_animator = GetComponentInChildren<Animator>();
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_Zoomout = m_Camera.fieldOfView;
			m_ZoomFOV = 20f;
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
            m_MouseLook.ZoomedSensitivity = new Vector2(m_ZoomAimSpeed, m_ZoomAimSpeed);
            m_StandHeight = GetComponent<CharacterController>().height;
            m_Rigidbody = GetComponent<Rigidbody>();
			m_scopeCamera = transform.Find("FirstPersonCharacter/rifle/metarig_001/canon_centered/Camera").GetComponent<Camera>();
		}


        // Update is called once per frame
        private void Update()
        {
            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = Input.GetButtonDown("Jump");
            }

            if (Input.GetButtonDown("Fire2"))
            {
                Aim(true);
            }
            if (Input.GetButtonUp("Fire2"))
            {
                Aim(false);
            }

            if (Input.GetButtonDown("Fire1") && m_fireRoutine == null)
            {
				m_fireRoutine = StartCoroutine(Fire());
            }

			float targetZoom = m_IsAiming ? m_ZoomFOV : m_Zoomout;
			if (m_Camera.fieldOfView != targetZoom)
			{
				m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, targetZoom, Time.deltaTime * m_ZoomSpeed);
			}

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            if (!m_isGrounded)
            {
                m_MoveDir.x += (1f - m_HitNormal.y) * m_HitNormal.x * (1f + 3f);
                m_MoveDir.z += (1f - m_HitNormal.y) * m_HitNormal.z * (1f + 3f);
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);
            m_isGrounded = (Vector3.Angle(Vector3.up, m_HitNormal) <= m_CharacterController.slopeLimit);

            ProgressStepCycle(speed);

            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            bool waswalking = m_IsWalking;

            #if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
            if ((Input.GetKey(KeyCode.LeftControl) ? true : Input.GetKey(KeyCode.C)))
            {
                Crouch();
            }
            else
            {
                UnCrouch();
            }
            #endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }
        }

        private void Crouch()
        {
            if (!m_IsCrouching)
            {
                GetComponent<CharacterController>().height -= m_CrouchDeltaHeight;
                transform.position -= new Vector3(0, m_CrouchDeltaHeight, 0);
                Camera.main.transform.localPosition -= new Vector3(0, m_CrouchDeltaHeight, 0);
                m_IsCrouching = true;
            }
        }

        private void UnCrouch()
        {
            if (m_IsCrouching)
            {
                GetComponent<CharacterController>().height = m_StandHeight;
                Camera.main.transform.localPosition += new Vector3(0, m_CrouchDeltaHeight, 0);
                m_IsCrouching = false;
            }
        }

        IEnumerator Fire()
        {
            // Play firing sound
            m_AudioSource.PlayOneShot(m_FireSound);
            // Preform raycast
            Ray ray = m_scopeCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
			Debug.DrawRay(ray.origin, ray.direction, Color.red, 2.0f);

			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Shootable")))
			{
				if (hit.collider.CompareTag("Citizen"))
				{
					hit.collider.transform.root.SendMessage("OnHit", m_ShotDamage);
				}
				if (hit.rigidbody != null)
				{
					hit.rigidbody.AddForceAtPosition(ray.direction * m_ShotForce, hit.point);
				}
				else
				{
					GameObject.Instantiate(m_BulletImpactPrefab, hit.point, Quaternion.identity);
				}
			}
            yield return new WaitForSeconds(0.1f);
			m_fireRoutine = null;
            // Recoil
            m_MouseLook.Recoil(m_Camera.transform, m_recoilForce);
		}

        private void Aim(bool aiming)
        {
            m_IsAiming = aiming;
            m_MouseLook.isAiming = aiming;
            m_animator.SetBool("Aim", aiming);
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }

        private void Pause()
        {

        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            m_HitNormal = hit.normal;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
