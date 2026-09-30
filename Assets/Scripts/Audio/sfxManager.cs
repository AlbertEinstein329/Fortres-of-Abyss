using UnityEngine;
using AbyssFirstPersonController;

[RequireComponent(typeof(AudioSource))]
public class sfxManager : MonoBehaviour
{
    private bool wasGrounded = true;

    [Header("References")]
    public AbyssFirstPersonController.FirstPersonController playerController;
    private AudioSource sfxSource;
    private AudioSource footstepsSource;
    private AudioSource slideSource;

    [Header("Audio Clips")]
    public AudioClip jumpclip;
    public AudioClip landClip;
    public AudioClip dashClip;
    public AudioClip slideClip;
    public AudioClip ledgeGrabClip;

    [Header("Footstep (Loop)")]
    public AudioClip footstepClip;
    [Tooltip("Footstep base volume")]
    [Range(0f, 1f)]
    public float footstepVolume = 0.5f;

    private void Awake()
    {
        sfxSource = GetComponent<AudioSource>();

        footstepsSource = gameObject.AddComponent<AudioSource>();
        footstepsSource.clip = footstepClip;
        footstepsSource.loop = true;
        footstepsSource.volume = footstepVolume;

        slideSource = gameObject.AddComponent<AudioSource>();
        slideSource.loop = true;
        slideSource.clip = slideClip;
    }

    private void OnEnable()
    {

        if (playerController != null)
        {
            playerController.OnPlayerJump += PlayJumpSound;
            playerController.OnPlayerDash += PlayDashSound;
            playerController.OnPlayerLedgeGrab += PlayLedgeGrabSound;
        }
    }
    private void OnDisable()
    {
        // Desuscribimos los métodos para evitar errores si el jugador se destruye
        if (playerController != null)
        {
            playerController.OnPlayerJump -= PlayJumpSound;
            playerController.OnPlayerDash -= PlayDashSound;
            playerController.OnPlayerLedgeGrab -= PlayLedgeGrabSound;
        }
    }

    private void Update()
    {
        if (playerController == null || footstepClip == null) return;

        ///--------------------------------------------------------
        
        //We read if the player is touching the ground in this exact frame
        bool isGroundedNow = playerController.isGrounded;
        //If in the previous frame it was NOT on the ground, and NOW it is, it means that it has just landed
        if (!wasGrounded && isGroundedNow)
        {
            PlayLandSound();
        }
        //We save the current state for the next frame.
        wasGrounded = isGroundedNow;
        ///--------------------------------------------------------

        //We get the exact name of the current state of the state machine
        string currentStateName = playerController.CurrentState.GetType().Name;
        bool isSliding = currentStateName == "PlayerSlidingState";
        bool isDashing = currentStateName == "PlayerDashState";
        
        ///------------------------------------------------------

        // Get the player's actual movement speed using the CharacterController
        float currentSpeed = playerController.characterController.velocity.magnitude;

        //We check if it is moving significantly and if it is on the ground
        bool isMovingOnGround = currentSpeed > 0.1f && playerController.isGrounded && !isSliding && !isDashing;

        if (isMovingOnGround && footstepClip != null)
        {
            if (!footstepsSource.isPlaying)
            {
                footstepsSource.Play();
            }

            footstepsSource.pitch = currentSpeed / playerController.walkSpeed; // Adjust pitch based on speed
        }
        else
        {
            if (footstepsSource.isPlaying)
            {
                footstepsSource.Stop();
            }
        }

        if (isSliding &&slideClip !=null)
        {
            if (!slideSource.isPlaying)
            {
                slideSource.Play();
            }
        }
        else
        {
            if (slideSource.isPlaying)
            {
                slideSource.Stop();
            }
        }
        ///------------------------------------------------------
    }

    private void PlayJumpSound()
    {
        if (jumpclip != null) sfxSource.PlayOneShot(jumpclip);
    }
    
    private void PlayLandSound()
    {
        if (landClip != null) sfxSource.PlayOneShot(landClip);
    }

    private void PlayDashSound()
    {
        if (dashClip != null) sfxSource.PlayOneShot(dashClip);
    }

    private void PlayLedgeGrabSound()
    {
        if (ledgeGrabClip != null) sfxSource.PlayOneShot(ledgeGrabClip);
    }
}