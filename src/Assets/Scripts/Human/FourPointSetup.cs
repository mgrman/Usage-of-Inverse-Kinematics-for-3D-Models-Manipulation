using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class FourPointSetup : MonoBehaviour, IBindToHumanRig
{
    public int _CalibrateWait;

    public BaseConnection _LeftHand;
    public BaseConnection _RightHand;
    public BaseConnection _Chest;
    public BaseConnection _BackOfNeck;

    public HumanIKControler _HumanControler;
    public HumanIKControler HumanControler
    {
        get { return _HumanControler; }
        set
        {
            _HumanControler = value;
            InitializeModel();
        }
    }

    public bool _InvertedX;
    public bool _InvertedZ;
    public bool _MirrorMode;

    public Vector3 _FallBackFrontDirection = new Vector3(0, 0, 1);
    public float _FallBackNeckThickness = 0;
    public float _StepSize = 1;
    public float _StepMinDistance = 1;
    public float _StepMinAngleDif = 45;
    public float _StepTime = 2;
    public float _StepHeight = 1;
    public float _CogDip = 1;

    #region accesors

    private Vector3 CogPosition { get { return _HumanControler.CogObj.position; } set { _HumanControler.CogObj.position = value; } }
    private Vector3 SpinePosition { get { return _HumanControler.SpineObj.position; } set { _HumanControler.SpineObj.position = value; } }
    private Vector3 ChestPosition { get { return _HumanControler.ChestObj.position; } set { _HumanControler.ChestObj.position = value; } }
    private Vector3 LookAtPosition { get { return _HumanControler.LookAtObj.position; } set { _HumanControler.LookAtObj.position = value; } }
    private Vector3 LeftFootPosition { get { return _HumanControler.LeftFootObj.position; } set { _HumanControler.LeftFootObj.position = value; } }
    private Vector3 LeftKneePosition { get { return _HumanControler.LeftKneeObj.position; } set { _HumanControler.LeftKneeObj.position = value; } }
    private Vector3 RightFootPosition { get { return _HumanControler.RightFootObj.position; } set { _HumanControler.RightFootObj.position = value; } }
    private Vector3 RightKneePosition { get { return _HumanControler.RightKneeObj.position; } set { _HumanControler.RightKneeObj.position = value; } }
    private Vector3 LeftHandPosition { get { return _HumanControler.LeftHandObj.position; } set { _HumanControler.LeftHandObj.position = value; } }
    private Vector3 LeftElbowPosition { get { return _HumanControler.LeftElbowObj.position; } set { _HumanControler.LeftElbowObj.position = value; } }
    private Vector3 RightHandPosition { get { return _HumanControler.RightHandObj.position; } set { _HumanControler.RightHandObj.position = value; } }
    private Vector3 RightElbowPosition { get { return _HumanControler.RightElbowObj.position; } set { _HumanControler.RightElbowObj.position = value; } }

    #endregion

    private float _armSpan_init;
    private float _armPosition_init;
    private Quaternion _armRotation_init;
    private Vector3 _chestPos_init;
    private Vector3 _cogPos_init;
    private Vector3 _leftFootPos_init;
    private Vector3 _rightFootPos_init;
    private Vector3 _leftKneePos_init;
    private Vector3 _rightKneePos_init;


    private Vector3 _lastChestOffset_step;
    private Coroutine _stepCoroutine_step;
    private Vector3 _cogStepOffset_step;

    private float _rotation_calib = 0;
    private float _scale_calib = 1;
    private Vector3 _offset_calib = new Vector3();

    private bool _justCalibrated_calib;
    private bool _avaitingCalibration_calib;
    private int _timeLeft_calib;

    private DetectHits _hitDetector;


    bool _lCon { get { return IsConnectorConnected(_LeftHand); } }
    bool _rCon { get { return IsConnectorConnected(_RightHand); } }
    bool _cCon { get { return IsConnectorConnected(_Chest); } }
    bool _nCon { get { return IsConnectorConnected(_BackOfNeck); } }


    Vector3 _lPos
    {
        get
        {

            if (_MirrorMode)
            {
                return CorrectVector(GetPositionFromConnector(_RightHand));
            }
            else
            {
                return CorrectVector(GetPositionFromConnector(_LeftHand));
            }
        }
    }
    Vector3 _rPos
    {
        get
        {

            if (_MirrorMode)
            {
                return CorrectVector(GetPositionFromConnector(_LeftHand));
            }
            else
            {
                return CorrectVector(GetPositionFromConnector(_RightHand));
            }
        }
    }
    Vector3 _cPos
    {
        get
        {
            return CorrectVector(GetPositionFromConnector(_Chest));
        }
    }
    Vector3 _nPos
    {
        get
        {
            return CorrectVector(GetPositionFromConnector(_BackOfNeck));
        }
    }
    Vector3 _middleOfNeckPos
    {
        get
        {
            Vector3 pos;
            if (_cCon && _nCon)
                pos = (_cPos + _nPos) / 2;
            else if (_cCon)
                pos = _cPos;
            else if (_nCon)
                pos = _nPos;
            else
            {
                var curArmSpan = (_lPos - _rPos).magnitude;
                curArmSpan = Mathf.Clamp(curArmSpan, 0, _armSpan_init);

                var offsetSize = Mathf.Sqrt(Mathf.Pow(_armSpan_init / 2, 2) - Mathf.Pow(curArmSpan / 2, 2));

                pos = (_lPos + _rPos) / 2 + (Quaternion.Euler(0, -90, 0) * (_lPos - _rPos).normalized) * offsetSize;
                pos.y = _chestPos_init.y;
            }
            return pos;
        }
    }


    void Awake()
    {
        _hitDetector = MonoBehaviour.FindObjectOfType<DetectHits>();
    }

    void Start()
    {
        InitializeModel();
    }

    void Update()
    {
        if (!_lCon || !_rCon)
            return;

        LeftHandPosition = GetLeftHandPos();

        RightHandPosition = GetRightHandPos();

        ChestPosition = GetChestPos();

        CogPosition = GetCoGPos();

        LeftElbowPosition = GetLeftElbowPos();

        RightElbowPosition = GetRightElbowPos();

        SpinePosition = GetSpinePos();


        var neckOffset = GetNeckCorrection();

        ChestPosition += neckOffset;
        CogPosition += neckOffset;
        SpinePosition += neckOffset;

        MakeStepIfNeeded();

        if (_justCalibrated_calib)
            Reset();

        _justCalibrated_calib = false;
    }

    void OnGUI()
    {
        float height = Screen.height / 20;

        float width = Screen.width / 5;


        if (_avaitingCalibration_calib)
        {
            GUI.enabled = false;
            if (GUI.Button(new Rect(Screen.width - width, Screen.height - height, width, height), string.Format("in {0} sec", _timeLeft_calib)))
            {

            }
        }
        else
        {
            GUI.enabled = true;
            if (GUI.Button(new Rect(Screen.width - width, Screen.height - height, width, height), "Calibrate characted"))
            {
                _timeLeft_calib = _CalibrateWait;
                _avaitingCalibration_calib = true;
                StartCoroutine(TimedCalibrate());
            }
        }


        GUI.enabled = true;
        if (GUI.Button(new Rect(0, Screen.height - height, width, height), "Mirror mode - " + (_MirrorMode ? "ON" : "OFF")))
        {
            _MirrorMode = !_MirrorMode;
        }
    }

    #region Update functions

    private Vector3 GetLeftHandPos()
    {
        return _lPos;
    }

    private Vector3 GetRightHandPos()
    {
        return _rPos;
    }

    private Vector3 GetLeftElbowPos()
    {
        var leftElbow = (LeftHandPosition + ChestPosition) / 2;
        var leftElbowOffset = Quaternion.Euler(0, -90, 0) * (_lPos - _middleOfNeckPos);
        return leftElbow + leftElbowOffset.normalized * 5;
    }

    private Vector3 GetRightElbowPos()
    {
        var rightElbow = (RightHandPosition + ChestPosition) / 2;
        var rightElbowOffset = Quaternion.Euler(0, 90, 0) * (_rPos - _middleOfNeckPos);
        return rightElbow + rightElbowOffset.normalized * 5;
    }

    private Vector3 GetChestPos()
    {
        return _middleOfNeckPos + _cogStepOffset_step; ;
    }

    private Vector3 GetCoGPos()
    {
        return _cogPos_init + (_middleOfNeckPos - _chestPos_init) + _cogStepOffset_step;
    }

    private Vector3 GetSpinePos()
    {
        Func<Vector3, Vector3, float> getDeterminant = (a, b) => Vector3.Dot(new Vector3(a.z, 0, -a.x), new Vector3(b.x, 0, b.z));

        Vector3 frontSideVector = _cCon && _nCon ? _cPos - _nPos : _FallBackFrontDirection;
        frontSideVector.y = 0;
        Vector3 rightSideVector = new Vector3(frontSideVector.z, 0, -frontSideVector.x);

        Vector3 spineOffset;
        if (_nCon && _cCon)
        {
            spineOffset = -frontSideVector;
        }
        else
        {
            var leftOffset = (LeftHandPosition - ChestPosition);
            var rightOffset = (RightHandPosition - ChestPosition);

            leftOffset.y = 0;
            rightOffset.y = 0;

            var spineFromElbows = (((LeftElbowPosition - ChestPosition) + (RightElbowPosition - ChestPosition)) / 2).normalized;

            var angle = Vector3.Angle(leftOffset, rightOffset);

            var deter = getDeterminant((LeftElbowPosition - ChestPosition), (RightElbowPosition - ChestPosition));
            var angleFromMain = Mathf.Acos(Vector3.Dot(spineFromElbows.normalized, frontSideVector.normalized)) / Mathf.PI * 180.0f;

            var deterFromMain = getDeterminant(spineFromElbows.normalized, rightSideVector.normalized);


            if (angle < 10)
            {
                var leftElbowOffset = Quaternion.Euler(0, -45, 0) * (_lPos - _middleOfNeckPos);
                var tempLeftElbow = (LeftHandPosition + ChestPosition) / 2 + leftElbowOffset.normalized * 5;

                var rightElbowOffset = Quaternion.Euler(0, 45, 0) * (_rPos - _middleOfNeckPos);
                var tempRightElbow = (RightHandPosition + ChestPosition) / 2 + rightElbowOffset.normalized * 5;


                deter = getDeterminant((tempLeftElbow - ChestPosition), (tempRightElbow - ChestPosition));

                spineFromElbows = (((tempLeftElbow - ChestPosition) + (tempRightElbow - ChestPosition)) / 2).normalized;


                angleFromMain = Mathf.Acos(Vector3.Dot(spineFromElbows.normalized, frontSideVector.normalized)) / Mathf.PI * 180.0f;
                deterFromMain = getDeterminant(spineFromElbows.normalized, rightSideVector.normalized);
            }

            //print(string.Format("deter         : {0}", deter));
            //print(string.Format("angle         : {0}", angle));
            //print(string.Format("angleFromMain : {0}", angleFromMain));
            //print(string.Format("deterFromMain : {0}", deterFromMain));

            //if (deter > 0 )
            //{

            //    spineFromElbows = -spineFromElbows;
            //}

            //if (deterFromMain > 0 && deter > 0)
            //{

            //    spineFromElbows = -spineFromElbows;
            //}

            if (deterFromMain > 0)
            {

                spineFromElbows = -spineFromElbows;
            }

            spineOffset = spineFromElbows;
        }

        spineOffset = spineOffset.normalized * 5;
        return (ChestPosition + CogPosition) / 2 + spineOffset;
    }

    private Vector3 GetNeckCorrection()
    {
        if (_cCon && _nCon || !_cCon && !_nCon)
            return new Vector3();

        var neckOffset = SpinePosition - ChestPosition;
        neckOffset.y = 0;
        neckOffset.Normalize();
        neckOffset *= _FallBackNeckThickness / 2;

        if (_cCon && !_nCon)
            return neckOffset;
        else if (!_cCon && _nCon)
            return -neckOffset;

        return new Vector3();
    }

    private void MakeStepIfNeeded()
    {
        Vector3 chestPos = ChestPosition;
        chestPos.y = 0;

        var feetPos = (LeftFootPosition + RightFootPosition) / 2;
        feetPos.y = 0;

        var feetDir = (LeftKneePosition + RightKneePosition) / 2 - feetPos;
        feetDir.y = 0;


        var chestDir = ChestPosition - SpinePosition;
        chestDir.y = 0;


        if (((chestPos - feetPos).magnitude > _StepMinDistance || Quaternion.Angle(Quaternion.LookRotation(chestDir), Quaternion.LookRotation(feetDir)) > _StepMinAngleDif) && _stepCoroutine_step == null)
        {
            var feetRightVector = RightFootPosition - LeftFootPosition;
            var moveDir = chestPos - feetPos;
            bool isStepLeftDir = Vector3.Dot(moveDir, feetRightVector) < 0;

            _stepCoroutine_step = StartCoroutine(MakeStep(feetPos, chestPos, feetDir, chestDir, isStepLeftDir));
        }
    }

    #endregion

    #region Making Step

    IEnumerator MakeStep(Vector3 fromPos, Vector3 toPos, Vector3 fromDir, Vector3 toDir, bool firstLeftFoot)
    {
        toPos = Vector3.ClampMagnitude(toPos - fromPos, _StepSize) + fromPos;
        //toPos = (toPos - fromPos).normalized * _StepSize + fromPos;

        float alpha = 0;

        while (alpha < 1)
        {
            SetFeet(fromPos, toPos, fromDir, toDir, alpha, firstLeftFoot);


            alpha += Time.deltaTime / _StepTime;
            yield return null;
        }
        SetFeet(fromPos, toPos, fromDir, toDir, 1, firstLeftFoot);

        _stepCoroutine_step = null;
    }

    private void SetFeet(Vector3 fromStep, Vector3 toStep, Vector3 fromDir, Vector3 toDir, float stepParameter, bool firstLeftFoot)
    {
        var difPos = (toStep - fromStep);

        //print(fromStep + dir * stepParameter);

        float sin = Mathf.Sin(stepParameter * Mathf.PI * 2);
        float cos = Mathf.Cos(stepParameter * Mathf.PI * 2);
        float leftStepHeight = sin > 0 ? sin : 0;
        float rightStepHeight = sin < 0 ? -sin : 0;

        leftStepHeight *= _StepHeight;
        rightStepHeight *= _StepHeight;


        var feetPos = (_leftFootPos_init + _rightFootPos_init) / 2;
        var feetPosPlanar = new Vector3(feetPos.x, 0, feetPos.z);

        var paramLeft = Mathf.Clamp(stepParameter * 2, 0, 1);
        var paramRight = Mathf.Clamp(stepParameter * 2 - 1f, 0, 1);

        var offsetLeft = fromStep + difPos * paramLeft - feetPosPlanar;
        var offsetRight = fromStep + difPos * paramRight - feetPosPlanar;

        var rotLeft = Quaternion.Slerp(Quaternion.LookRotation(fromDir), Quaternion.LookRotation(toDir), paramLeft);
        var rotRight = Quaternion.Slerp(Quaternion.LookRotation(fromDir), Quaternion.LookRotation(toDir), paramRight);

        var dirLeft = rotLeft * Vector3.forward;
        var dirRight = rotRight * Vector3.forward;

        var leftFootDifVector = _leftFootPos_init - feetPosPlanar;
        leftFootDifVector.y = 0;

        var rightFootDifVector = _rightFootPos_init - feetPosPlanar;
        rightFootDifVector.y = 0;

        if (firstLeftFoot)
        {
            LeftFootPosition = feetPos + rotLeft * leftFootDifVector + offsetLeft + Vector3.up * leftStepHeight;
            RightFootPosition = feetPos + rotLeft * rightFootDifVector + offsetRight + Vector3.up * rightStepHeight;

            LeftKneePosition = LeftFootPosition + dirLeft;
            RightKneePosition = RightFootPosition + dirRight;
        }
        else
        {
            RightFootPosition = feetPos + rotLeft * rightFootDifVector + offsetLeft + Vector3.up * leftStepHeight;
            LeftFootPosition = feetPos + rotLeft * leftFootDifVector + offsetRight + Vector3.up * rightStepHeight;

            var dir = Quaternion.Slerp(Quaternion.LookRotation(fromDir), Quaternion.LookRotation(toDir), stepParameter) * Vector3.forward;
            RightKneePosition = RightFootPosition + dirLeft;
            LeftKneePosition = LeftFootPosition + dirRight;
        }

        var cogParam = (cos / 2.0f) - 0.5f;
        _cogStepOffset_step = new Vector3(0, cogParam * _CogDip, 0);

    }

    #endregion

    #region Calibration

    private void InitializeModel()
    {
        _armSpan_init = (LeftHandPosition - ChestPosition).magnitude + (RightHandPosition - ChestPosition).magnitude;

        _armPosition_init = (LeftHandPosition.y + ChestPosition.y + RightHandPosition.y) / 3;

        _chestPos_init = ChestPosition;

        _cogPos_init = CogPosition;
        _leftFootPos_init = LeftFootPosition;
        _rightFootPos_init = RightFootPosition;
        _leftKneePos_init = LeftKneePosition;
        _rightKneePos_init = RightKneePosition;

        var rightArmVector = RightHandPosition - LeftHandPosition;
        rightArmVector.y = 0;
        _armRotation_init = Quaternion.FromToRotation(new Vector3(1, 0, 0), rightArmVector);
    }

    private void Calibrate()
    {
        var lPos = _LeftHand._LastPosition;
        var rPos = _RightHand._LastPosition;
        var cPos = _Chest._LastPosition;
        var nPos = _BackOfNeck._LastPosition;

        Vector3 middleOfNeckPos;
        if (_cCon && _nCon)
            middleOfNeckPos = (cPos + nPos) / 2;
        else if (_cCon)
            middleOfNeckPos = cPos;
        else if (_nCon)
            middleOfNeckPos = nPos;
        else
            middleOfNeckPos = (lPos + rPos) / 2;


        _scale_calib = _armSpan_init / ((lPos - middleOfNeckPos).magnitude + (rPos - middleOfNeckPos).magnitude);

        var spineOffset = Quaternion.Euler(0, 90, 0) * (rPos - lPos).normalized / _scale_calib;
        Vector3 offset = new Vector3();
        if (_cCon && !_nCon)
            offset = spineOffset * _FallBackNeckThickness / 2;
        else if (!_cCon && _nCon)
            offset = -spineOffset * _FallBackNeckThickness / 2;

        middleOfNeckPos += offset;


        _offset_calib = -middleOfNeckPos;

        var rightArmVector = rPos - lPos;
        rightArmVector.y = 0;
        var armRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), rightArmVector);
        _rotation_calib = _armRotation_init.eulerAngles.y - armRotation.eulerAngles.y;
        _justCalibrated_calib = true;

        if (_hitDetector != null)
            _hitDetector.Reset();
    }

    Vector3 CorrectVector(Vector3 pos)
    {

        Vector3 newPos = new Vector3();
        newPos.x = (pos.x + _offset_calib.x) * _scale_calib;
        newPos.y = (pos.y + _offset_calib.y) * _scale_calib;
        newPos.z = (pos.z + _offset_calib.z) * _scale_calib;

        newPos.x *= _InvertedX ? -1 : 1;
        newPos.z *= _InvertedZ ? -1 : 1;

        if (_MirrorMode)
        {
            newPos.x *= -1;
        }

        var quat = Quaternion.Euler(0, -_rotation_calib, 0);

        newPos = quat * (newPos);

        newPos.x += _chestPos_init.x;
        newPos.y += _chestPos_init.y;
        newPos.z += _chestPos_init.z;
        return newPos;
        //return new Vector3 (x, y, z);
        //return _correctionMatrix.MultiplyPoint( pos);
    }

    IEnumerator TimedCalibrate()
    {
        while (_timeLeft_calib > 0)
        {
            yield return new WaitForSeconds(1);
            _timeLeft_calib--;
        }
        Calibrate();
        _avaitingCalibration_calib = false;
    }

    #endregion

    #region Utils

    private void Reset()
    {
        Vector3 chestOffset = ChestPosition;
        chestOffset.y = 0;


        var chestDir = ChestPosition - SpinePosition;
        chestDir.y = 0;

        if (_stepCoroutine_step != null)
            StopCoroutine(_stepCoroutine_step);
        _stepCoroutine_step = null;
        SetFeet(chestOffset, chestOffset, chestDir, chestDir, 1, true);
        _lastChestOffset_step = chestOffset;
    }

    private bool IsConnectorConnected(BaseConnection con)
    {
        return con != null && con.enabled && con._Connected;
    }

    private Vector3 GetPositionFromConnector(BaseConnection con)
    {
        return IsConnectorConnected(con) ? con._LastPosition : new Vector3();
    }

    #endregion
}
