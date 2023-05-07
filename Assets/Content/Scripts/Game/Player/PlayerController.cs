using System;
using Content.Scripts.Game.GameStateService;
using Content.Scripts.Game.InputService;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Game.SettingsService;
using UnityEngine;

namespace Content.Scripts.Game.Player
{
    public class PlayerController : IUpdateData
    {
        public enum MechState
        {
            Normal,
            Fly,
            FuelWait
        }

        private bool enabled = true;

        private IInputService inputService;
        private ISettingsService settingsService;
        private Rigidbody rb;
        private MechObject.MechData values;
        private MechBuilder.MechPartsData parts;
        private MechBuilder builder;
        private PlayerBuilder player;

        private float yLookAngle = 0;
        private float camerasOffcet = 0;
        private MechState mechState;
        private MechData valuesData;
        private IStateService stateService;


        public event Action OnFlyStart, OnFlyEnd;


        private const float NITRO_SPEED = 30;
        private const float NITRO_SPEED_ADD = 20;
        private const float NITRO_TO_START_FLY = 0.75f;



        public bool Enabled => enabled;

        public float YLookAngle => yLookAngle;


        public PlayerController(IInputService inputService, ISettingsService settingsService, Rigidbody rb, PlayerBuilder player, MechData valuesData, IStateService stateService)
        {
            this.stateService = stateService;
            this.valuesData = valuesData;
            this.inputService = inputService;
            this.settingsService = settingsService;
            this.rb = rb;
            this.values = player.MechObject.MechValues;
            this.builder = player.MechBuilder;
            this.parts = builder.PartsData;
            this.player = player;


            Cursor.lockState = CursorLockMode.Locked;

            ConfigureInput();
        }


        public void SetEnabled(bool state) => enabled = state;

        public void Disable()
        {
            inputService.OnMove -= Moving;
            inputService.OnMouse -= Look;
        }



        private void ConfigureInput()
        {
            inputService.OnMove += Moving;
            inputService.OnMouse += Look;
            if (builder.PartsData.Thrusters.Count != 0)
            {
                inputService.OnJumpStart += FlyStart;
                inputService.OnJumpEnd += FlyEnd;
                inputService.OnJump += FlyProcess;
            }
        }


        private void FlyProcess()
        {
            if (mechState == MechState.Fly)
            {
                valuesData.AddFuel(-Time.deltaTime * NITRO_SPEED);
                rb.AddForce(Vector3.up * values.WorldParameters.FlyAcceleration * Time.deltaTime, ForceMode.Force);
                if (valuesData.Fuel <= 0)
                {
                    ChangeMechState(MechState.FuelWait);
                    OnFlyEnd?.Invoke();
                }
            }
        }

        private void FlyEnd()
        {
            if (mechState != MechState.FuelWait)
            {
                ChangeMechState(MechState.Normal);
            }

            OnFlyEnd?.Invoke();
        }

        private void FlyStart()
        {
            if (!enabled) return;
            if (mechState == MechState.FuelWait)
            {
                if (valuesData.Fuel < valuesData.MaxFuel * NITRO_TO_START_FLY)
                {
                    return;
                }
            }

            OnFlyStart?.Invoke();
            ChangeMechState(MechState.Fly);
        }



        public void Update()
        {
            if (!enabled) return;
            if (valuesData.Fuel < valuesData.MaxFuel && mechState != MechState.Fly && !player.IsFly)
            {
                valuesData.AddFuel(Time.deltaTime * NITRO_SPEED_ADD);
            }
        }

        private void ChangeMechState(MechState state) => mechState = state;

        private void Look(Vector2 obj)
        {
            if (!enabled || !stateService.IsCanPlayerPlay) return;
            LookTorso(obj);
            LookVertical(obj);
            LookCameras();
        }

        private void LookVertical(Vector2 obj)
        {
            var rotateDelta = settingsService.SettingsObject.MouseSensitivity.y * Time.deltaTime * -obj.y;
            yLookAngle = YLookAngle + rotateDelta;
            yLookAngle = Mathf.Clamp(YLookAngle, -20, 25);
        }

        private void LookCameras()
        {
            camerasOffcet = Mathf.Lerp(camerasOffcet, 0, Time.deltaTime * 20);

            player.MainCamera.transform.localEulerAngles = new Vector3(YLookAngle / 2f, camerasOffcet, 0);
            for (int i = 0; i < builder.Cameras.Count; i++)
            {
                builder.Cameras[i].transform.localEulerAngles = new Vector3(YLookAngle / 2f, camerasOffcet, 0);
            }
        }

        private void LookTorso(Vector2 obj)
        {
            var rotateDelta = values.TorsoParameters.HorizontalSpeed * Time.deltaTime * obj.x * settingsService.SettingsObject.MouseSensitivity.x;
            camerasOffcet += rotateDelta;
            parts.YRotator.Rotate(Vector3.up * rotateDelta, Space.Self);
            ClampTorso(-rotateDelta);
            parts.Torso.localEulerAngles = new Vector3(YLookAngle * 2f, 0, 0);
        }

        private void Moving(Vector2 obj)
        {
            if (!enabled || !stateService.IsCanPlayerPlay) return;

            var rotateDelta = values.LegsParameters.RotateSpeed * Time.deltaTime * obj.x;
            var force = values.LegsParameters.MoveSpeed * Time.deltaTime * Mathf.Clamp(obj.y, -50, 50);
            if (mechState != MechState.Fly && !player.IsFly)
            {
                rb.AddForce(parts.Plevis.forward * force, ForceMode.Force);
            }
            else if (player.IsFly && mechState == MechState.Fly)
            {
                rb.AddForce(parts.YRotator.forward * force, ForceMode.Force);
            }

            parts.Plevis.Rotate(Vector3.up * rotateDelta, Space.Self);
            parts.YRotator.Rotate(Vector3.up * rotateDelta, Space.Self);
        }

        private void ClampTorso(float rotateDelta)
        {
            if (Vector3.Dot(parts.Plevis.forward, parts.Torso.forward) < 0)
            {
                parts.YRotator.Rotate(Vector3.up * rotateDelta, Space.Self);
            }
        }
    }
}
