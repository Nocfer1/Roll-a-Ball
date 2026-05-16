using System.Collections;
using System.Collections.Generic;
using Audio;
using Managers;
using UnityEngine;
using PlayerFolder;

namespace Runner
{
    public class RunnerPlayerHealth : MonoBehaviour
    {
        public int maxLives = 3;
        public int currentLives;

        public int playerID = 1;
        public bool isDead = false;
        public int hitsReceived = 0;

        public float deathProgressPercent = -1f;

        [Header("Damage Protection")]
        public float invulnerabilityTime = 0.6f;

        [Header("Hit Feedback")]
        public Color hitColor = Color.red;
        public float hitFlashDuration = 0.15f;
        public int hitFlashCount = 2;

        [Header("Death Visual")]
        public bool hidePlayerOnDeath = true;
        public GameObject visualRoot;

        private bool isInvulnerable = false;

        private Renderer[] renderers;
        private readonly List<Material> cachedMaterials = new List<Material>();
        private readonly List<Color> originalColors = new List<Color>();

        private Coroutine flashCoroutine;

        private void Start()
        {
            currentLives = maxLives;
            isDead = false;
            isInvulnerable = false;
            hitsReceived = 0;
            deathProgressPercent = -1f;

            CacheMaterials();
        }

        private void CacheMaterials()
        {
            renderers = GetComponentsInChildren<Renderer>();

            cachedMaterials.Clear();
            originalColors.Clear();

            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.materials;

                foreach (Material mat in materials)
                {
                    cachedMaterials.Add(mat);

                    if (mat.HasProperty("_BaseColor"))
                        originalColors.Add(mat.GetColor("_BaseColor"));
                    else if (mat.HasProperty("_Color"))
                        originalColors.Add(mat.GetColor("_Color"));
                    else
                        originalColors.Add(Color.white);
                }
            }
        }

        public void TakeDamage(int damage)
        {
            if (isDead) return;
            if (isInvulnerable) return;

            isInvulnerable = true;

            hitsReceived += 1;
            currentLives -= damage;

            GameAudioManager.Instance?.PlayRunnerHit();
            
            if (currentLives <= 0)
            {
                currentLives = 0;
                StartCoroutine(FlashThenDie());
            }
            else
            {
                StartHitFlash();
                StartCoroutine(InvulnerabilityRoutine());
            }
        }

        private IEnumerator InvulnerabilityRoutine()
        {
            yield return new WaitForSeconds(invulnerabilityTime);
            isInvulnerable = false;
        }

        private void StartHitFlash()
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);

            flashCoroutine = StartCoroutine(HitFlashRoutine());
        }

        private IEnumerator HitFlashRoutine()
        {
            for (int i = 0; i < hitFlashCount; i++)
            {
                SetPlayerColor(hitColor);
                yield return new WaitForSeconds(hitFlashDuration);

                RestoreOriginalColors();
                yield return new WaitForSeconds(hitFlashDuration);
            }

            flashCoroutine = null;
        }

        private IEnumerator FlashThenDie()
        {
            isDead = true;

            if (RunnerGameManager.Instance != null)
            {
                deathProgressPercent = RunnerGameManager.Instance.GetCurrentProgressPercent();
            }

            SetPlayerColor(hitColor);
            yield return new WaitForSeconds(0.25f);

            Die();
        }

        private void SetPlayerColor(Color color)
        {
            foreach (Material mat in cachedMaterials)
            {
                if (mat == null) continue;

                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", color);
                else if (mat.HasProperty("_Color"))
                    mat.SetColor("_Color", color);
            }
        }

        private void RestoreOriginalColors()
        {
            for (int i = 0; i < cachedMaterials.Count; i++)
            {
                Material mat = cachedMaterials[i];
                if (mat == null) continue;

                Color originalColor = originalColors[i];

                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", originalColor);
                else if (mat.HasProperty("_Color"))
                    mat.SetColor("_Color", originalColor);
            }
        }

        private void Die()
        {
            RunnerPlayerController runnerController = GetComponent<RunnerPlayerController>();
            if (runnerController != null)
                runnerController.enabled = false;

            CharacterController characterController = GetComponent<CharacterController>();
            if (characterController != null)
                characterController.enabled = false;

            Animator animator = GetComponentInChildren<Animator>();
            if (animator != null)
                animator.enabled = false;

            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }

            if (hidePlayerOnDeath)
            {
                if (visualRoot != null)
                {
                    visualRoot.SetActive(false);
                }
                else
                {
                    Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in allRenderers)
                    {
                        renderer.enabled = false;
                    }
                }
            }
        }
    }
}