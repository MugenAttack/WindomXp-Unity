using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MA_Runner
{
    public windomAnimation[] anim;
    public windomAnimation sAnim; //script anim;
    public int scriptIndex = 0;
    public int scriptTime = 0;
    public int frameIndex = 0;
    public float frameTime = 0;
    public bool loop;
    public bool animeEnd = false;
    public float blendWeight;

    public MA_Runner(windomAnimation ani, windomAnimation sAni = null, bool _loop = false)
    {
        anim = new windomAnimation[] { ani };
        if (sAni != null)
            sAnim = sAni;
        else
            sAnim = ani;
        blendWeight = 0;
        loop = _loop;
    }

    public MA_Runner(windomAnimation[] animations, float _blendWeight,  windomAnimation sAni = null, bool _loop = false)
    {
        if (sAni != null)
            sAnim = sAni;
        else
            sAnim = animations[0];

        int frameLength = animations[0].frames.Count;
        for (int i = 1; i < animations.Length; i++)
        {
            if (animations[i].frames.Count != frameLength)
            {
                frameLength = -1;
                break;
            }
        }

        if (frameLength != -1)
        {
            anim = animations;
            blendWeight = _blendWeight;
        }
        else
        {
            anim = new windomAnimation[] { animations[0] };
            blendWeight = 0;
        }

        loop = _loop;
    }

    public void changeAnim(windomAnimation ani)
    {
        if (anim.Length == 1)
            anim = new windomAnimation[] { ani };
    }

    public void changeAnim(windomAnimation[] ani)
    {
        if (anim.Length == ani.Length)
            anim = ani;
    }
    public void Update()
    {
        if (!animeEnd)
        {
            scriptTime++;

            if (scriptTime >= sAnim.scripts[scriptIndex].unk)
            {

                scriptIndex++;
                scriptTime = 0;
                if (scriptIndex >= sAnim.scripts.Count)
                {
                    if (loop)
                    {
                        scriptIndex = 0;
                        frameTime = 0;
                        frameIndex = 0;
                    }
                    else
                        animeEnd = true;

                }
            }

            if (!animeEnd)
            {
                frameTime += sAnim.scripts[scriptIndex].time;
                if (frameTime >= 1)
                {
                    frameIndex++;
                    frameTime = frameTime - 1f;
                }
            }
        }
    }

    public hod2v1_Part getMT(int partID)
    {
        if (anim.Length > 1)
        {
            hod2v1_Part a = anim[0].interpolatePart(frameIndex, partID, frameTime);
            hod2v1_Part b = anim[1].interpolatePart(frameIndex, partID, frameTime);
            return MechaAnimator.InterpolateTransform(a, b, blendWeight);
        }

        return anim[0].interpolatePart(frameIndex, partID, frameTime);
    }

    public int getScriptTransitionLength()
    {
        return sAnim.scripts[scriptIndex].unk;
    }
}

public class MechaAnimator : MonoBehaviour
{   
    [Header("Play Data")]
    public bool play = false;
    float fps = 1 / 30;
    float time = 0;
    public MA_Runner runner;
    public MA_Runner prevRunner;
    public float transition = 0;
    public float transitionSpeed = 0.1f;
    public bool playTop = false;
    public MA_Runner UpperOverride;
    public MA_Runner prevUpperOverride;
    public float uTransition = 0;
    public float uTransitionSpeed = 0.1f;
    public RoboStructure structure;
    public scriptInterpreter scriptRunner;
    public MechaMovement movement;
    public bool resetMovementPerScript = false;
    // Start is called before the first frame update
    void Start()
    {
       //scriptRunner = new scriptInterpreter();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (play && !runner.animeEnd)
        {

            if (runner.scriptTime == 0)
            { 
                if (resetMovementPerScript)
                    movement.moveSpeed = Vector3.zero;
                for (int i = 0; i < movement.Thrusters.Count; i++)
                    movement.Thrusters[i].SetActive(false);
                script s = runner.sAnim.scripts[runner.scriptIndex];
                while (s.unk == 0 && s.time == 0 && runner.scriptIndex < runner.sAnim.scripts.Count - 1)
                {
                    scriptRunner.runScript(s.squirrel);
                    runner.scriptIndex++;
                    s = runner.sAnim.scripts[runner.scriptIndex];
                }
                scriptRunner.runScript(s.squirrel);
            }

            if (playTop && UpperOverride.scriptTime == 0)
                scriptRunner.runScript(UpperOverride.sAnim.scripts[UpperOverride.scriptIndex].squirrel);

            time += Time.deltaTime;
            //if (time >= fps)
            //{
                //isUpdated = true;
                time = 0;
                runner.Update();
                if(playTop)
                    UpperOverride.Update();

                //interpolate between current frame and next frame
                if (runner.frameIndex < runner.anim[0].frames.Count - 1)
                {
                    for (int i = 1; i < structure.parts.Count; i++)
                    {
                        GameObject go = structure.parts[i];
                        if (go != null)
                        {
                            hod2v1_Part mt = new hod2v1_Part();
                            if (playTop && structure.isTop[i])
                            {
                                if (prevUpperOverride != null && uTransition < 1)
                                    mt = InterpolateTransform(prevUpperOverride.getMT(i), UpperOverride.getMT(i), uTransition);
                                else
                                    mt = UpperOverride.getMT(i);
                            }
                            else
                            {
                                if (prevRunner != null && transition < 1)
                                    mt = InterpolateTransform(prevRunner.getMT(i), runner.getMT(i), transition);
                                else
                                    mt = runner.getMT(i);
                            }
                                go.transform.localPosition = mt.position;
                                go.transform.localRotation = mt.rotation;
                                go.transform.localScale = mt.scale;
                                if (mt.scale.x + mt.scale.y + mt.scale.z < 0.5)
                                    Debug.Log("Bug");
                            }
                    }
                    if (prevRunner != null && transition < 1)
                        transition += transitionSpeed;
                    if (prevUpperOverride != null && uTransition < 1)
                        uTransition += uTransitionSpeed;
            }
            //}
            //else
            //	isUpdated = false;

        }
    }
    public void run(int animID, bool _loop = false)
    {
        prevRunner = runner;
        runner = new MA_Runner(structure.ani.animations[animID], null, _loop);
        play = true;
        transition = 0;

    }

    public void run(int[] animIDs, float blend, bool _loop = false)
    {
        windomAnimation[] animations = new windomAnimation[animIDs.Length]; 
        for (int i = 0; i < animIDs.Length; i++)
            animations[i] = structure.ani.animations[animIDs[i]];
        prevRunner = runner;
        runner = new MA_Runner(animations, blend, null, _loop);
        play = true;
        transition = 0;

    }

    public void run(windomAnimation animID, bool _loop = false)
    {
        prevRunner = runner;
        runner = new MA_Runner(animID, null, _loop);
        play = true;
        transition = 0;

    }

    public void run(windomAnimation[] animIDs, float blend, bool _loop = false)
    {
        prevRunner = runner;
        runner = new MA_Runner(animIDs, blend, null, _loop);
        play = true;
        transition = 0;

    }

    public void runUpper(int animID, bool _loop = false)
    {
        if (playTop)
            prevUpperOverride = UpperOverride;
        else
            prevUpperOverride = runner;
        UpperOverride = new MA_Runner(structure.ani.animations[animID], null, _loop);
        playTop = true;
        uTransition = 0;
    }
    public void changeAnim(int animID)
    {
        runner.changeAnim(structure.ani.animations[animID]);
    }

    public void changeAnim(int[] animIDs)
    {
        windomAnimation[] animations = new windomAnimation[animIDs.Length];
        for (int i = 0; i < animIDs.Length; i++)
            animations[i] = structure.ani.animations[animIDs[i]];
        runner.changeAnim(animations);
    }
    public bool isEnded()
    {
        return runner.animeEnd;
    }

    public bool isUpperEnded()
    {
        return UpperOverride.animeEnd;
    }
    public static hod2v1_Part InterpolateTransform(hod2v1_Part a, hod2v1_Part b, float t)
    {
        hod2v1_Part iMT = new hod2v1_Part();
        if (a.rotation.x + a.rotation.y + a.rotation.z + a.rotation.z == 0)
            a.rotation = Quaternion.identity;
        if (b.rotation.x + b.rotation.y + b.rotation.z + b.rotation.z == 0)
            b.rotation = Quaternion.identity;

        iMT.position = Vector3.Lerp(a.position, b.position, t);
        iMT.rotation = Quaternion.Lerp(a.rotation, b.rotation, t);
        iMT.scale = Vector3.Lerp(a.scale, b.scale, t);

        return iMT;
    }

    public int scriptsTillEnd()
    {
        return runner.sAnim.scripts.Count - runner.scriptIndex;
    }

    public int getScriptTransitionLength()
    {
        return runner.getScriptTransitionLength();
    }
}
