using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Spine
{
    public class AutoSineSwing : MonoBehaviour
    {
        public int selfMode;
        public float selfRotateCenter;
        public float selfRotateTime;
        public float selfRotateRange;
        public float selfRotateOffset;
        public float selfChildOffset;
        public float selfSpring;
        public float selfAffectByLevel;
        public float selfSpringLevel;
        public string selfEndBoneName;
        public float selfMoveXFreq = 1;
        public float selfMoveXAmp = 0;
        public float selfMoveXOctaves = 0;
        public float selfMoveXDelay = 0;
        public float selfMoveXCenter = 0;
        public float selfMoveXSeed;
        public float selfMoveYFreq = 1;
        public float selfMoveYAmp = 0;
        public float selfMoveYOctaves = 0;
        public float selfMoveYDelay = 0;
        public float selfMoveYCenter = 0;
        public bool selfMoveYSameAsX = false;
        public float selfScaleXFreq = 1;
        public float selfScaleXAmp = 0;
        public float selfScaleXOctaves = 0;
        public float selfScaleXDelay = 0;
        public float selfScaleXCenter = 0;
        public float selfScaleYFreq = 1;
        public float selfScaleYAmp = 0;
        public float selfScaleYOctaves = 0;
        public float selfScaleYDelay = 0;
        public float selfScaleYCenter = 0;
        public bool selfScaleYSameAsX = false;
        public float selfRotateSpeed = 0;
        public float selfRotateFreq = 1;
        public float selfRotateAmp = 0;
        public float selfRotateOctaves = 0;
        public float selfRotateDelay = 0;
        public bool selfRotateFollowEnable = false;
        public float selfRotateFollowLimit = 0;
        public float selfRotateFollowSpeed = 0.1f;
        public float selfRotateFollowFlip = 0;
        public float selfRotateFollowXMax = 20f;
        public float selfRotateFollowYMax = 20f;
        // sine mode
        public float selfScaleXRange = 0f;
        public float selfScaleXOffset = 0f;
        public float selfScaleXChildOffset = 0.25f;
        public float selfScaleXSpring = 0f;
        public float selfScaleXTime = 2f;
        public float selfScaleXAffectByLevel = 0.1f;
        public float selfScaleYRange = 0f;
        public float selfScaleYOffset = 0f;
        public float selfScaleYChildOffset = 0.25f;
        public float selfScaleYSpring = 0f;
        public float selfScaleYTime = 2f;
        public float selfScaleYAffectByLevel = 0.1f;
        public bool selfSinScaleXSameAsY = false;
        //mode = 4
        public float selfDelay = 0.1f;
        public float selfLimitRange = 0;
        public float selfSpeed = 0;

        public class CustomBone
        {
            public CustomBone parent;
            public string name;
            public Transform transform;
            public Quaternion initalRotation;
            public List<CustomBone> children;
            //Local Position x
            //public float x;
            //Local Position y
            //public float y;

            //Local Scale
            //public float scaleX;
            //public float scaleY;
            public float initX;
            public float initY;
            //Quaternion to float
            public float initRotation;
            public float initWorldX;
            public float initWorldY;
            public float initScaleX;
            public float initScaleY;
            public float autoMovePrevWorldX;
            public float autoMovePrevWorldY;
            public float autoMoveSpeedX;
            public float autoMoveSpeedY;
            public float autoMoveFriction;
            public float followRotation;
            public float elasticSpeedX;
            public float elasticSpeedY;


            public float tailAutoMovePrevWorldX = 0;
            public float tailAutoMovePrevWorldY = 0;

            public Spine.Bone boneRef;
        }

        public Transform rootBoneTransform;

        public CustomBone rootBone;

        public float fTime;

        public SkeletonAnimation skeletonAnimationRef;

        // Start is called before the first frame update
        void Start()
        {
            if(!selfMoveYSameAsX)
                selfMoveYSameAsX = selfMoveXFreq == selfMoveYFreq && selfMoveXAmp == selfMoveYAmp && selfMoveXOctaves == selfMoveYOctaves && selfMoveXDelay == selfMoveYDelay && selfMoveXCenter == selfMoveYCenter;

            if(!selfScaleYSameAsX)
                selfScaleYSameAsX = selfScaleXFreq == selfScaleYFreq && selfScaleXAmp == selfScaleYAmp && selfScaleXOctaves == selfScaleYOctaves && selfScaleXDelay == selfScaleYDelay && selfScaleXCenter == selfScaleYCenter;

            selfRotateFollowEnable = 0 != selfRotateFollowLimit;

            if(!selfSinScaleXSameAsY)
                selfSinScaleXSameAsY = selfScaleXRange == selfScaleYRange && selfScaleXOffset == selfScaleYOffset && selfScaleXChildOffset == selfScaleYChildOffset && selfScaleXSpring == selfScaleYSpring && selfScaleXAffectByLevel == selfScaleYAffectByLevel;

            selfMoveXSeed = selfMoveXSeed == 0 ? Mathf.Floor(1e4f * Random.Range(0f, 1f)):selfMoveXSeed;

            StartAutoSwing();
        }

        public void StartAutoSwing()
        {
            if (rootBoneTransform == null)
            {
                rootBoneTransform = transform;
            }

            if (rootBoneTransform != null)
            {
                rootBone = PackTransform(rootBoneTransform);
            }

            if(skeletonAnimationRef != null)
            {
                skeletonAnimationRef.UpdateLocal += OnNeedToSwing;
            }
        }

        public CustomBone PackTransform(Transform rootTransform)
        {
            CustomBone root = null;

            if (rootTransform != null)
            {
                root = new CustomBone();
                root.parent = null;
                root.name = rootTransform.name;
                root.transform = rootTransform;
                root.initX = rootTransform.localPosition.x;
                root.initY = rootTransform.localPosition.y;
                root.initWorldX = rootTransform.position.x;
                root.initWorldY = rootTransform.position.y;
                root.initScaleX = rootTransform.localScale.x;
                root.initScaleY = rootTransform.localScale.y;
                root.initalRotation = rootTransform.rotation;
                root.boneRef = rootTransform.GetComponent<Spine.Unity.SkeletonUtilityBone>().bone;
                root.initRotation = root.boneRef.Rotation;

                List<CustomBone> result = new List<CustomBone>();

                foreach (Transform tr in rootTransform)
                {
                    result.Add(PackTransform(tr));
                }

                root.children = result;

                if(root.children.Count == 0)
                {
                    root.tailAutoMovePrevWorldX = root.boneRef.X * root.boneRef.B + root.boneRef.WorldX;
                    root.tailAutoMovePrevWorldY = root.boneRef.Y * root.boneRef.D + root.boneRef.WorldY;
                }
            }

            return root;
        }

        // Update is called once per frame
        public void OnNeedToSwing(ISkeletonAnimation animatedSkeletonComponent)
        {
            if (rootBone != null)
            {
                switch (selfMode)
                {
                    case 1:
                        UpdateSineMode(Time.time, rootBone, 0, 0.8f, selfEndBoneName);
                        break;
                    case 3:
                        UpdateMode3Func(Time.time,0.0001f);
                        break;
                    case 4:
                        Vector3 scale = this.rootBone.transform.localScale;
                        float dimension = scale.x * scale.y;
                        UpdateSpringMagic(rootBone, 0.5f, 0, 1f, dimension > 0 ? -1 : 1);
                        break;
                }
            }
        }

        public void ResetMoveFriction(CustomBone root)
        {
            if(root.children.Count > 0)
            {
                foreach(var c in root.children)
                {
                    ResetMoveFriction(c);
                    c.autoMoveFriction = 0;
                }
            }
            //root.autoMoveFriction = 0;
            root.autoMoveFriction = 0;
        }

        void UpdateSineMode(float time, CustomBone boneTransform, int index, float mixParam, string endBoneName)
        {
            if (!boneTransform.name.Equals(endBoneName))
            {
                float rootScaleX = boneTransform.boneRef.ScaleX;
                float rootScaleY = boneTransform.boneRef.ScaleY;

                float offset = Mathf.Sin((selfRotateOffset - Mathf.Pow(selfChildOffset * index, 1 + selfSpring) + time)
                * Mathf.PI * 2 / selfRotateTime)
                * selfRotateRange * Mathf.Pow(1 + index * selfAffectByLevel, 1 + selfSpringLevel) + selfRotateCenter;

                boneTransform.boneRef.Rotation = Mathf.Lerp(boneTransform.boneRef.Rotation, boneTransform.initRotation + offset, mixParam);

                //Quaternion newRotation = boneTransform.initalRotation * Quaternion.Euler(0, 0, offset);

                /*boneTransform.transform.rotation = Quaternion.Lerp(boneTransform.transform.rotation,
                newRotation,
                mixParam);*/

                float o = 0; 
                if(0 != selfScaleYRange)
                {
                    o = Mathf.Sin((selfScaleYOffset - Mathf.Pow(selfScaleYChildOffset * index, 1 + selfScaleYSpring) + time) * Mathf.PI * 2 / selfScaleYTime) * selfScaleYRange * Mathf.Pow(1 + index * selfScaleYAffectByLevel, 1 + selfSpringLevel) + selfScaleYCenter;
                    rootScaleY = Mathf.Lerp(rootScaleY, boneTransform.initScaleY + o, mixParam);
                    if(selfSinScaleXSameAsY)
                        rootScaleX = Mathf.Lerp(rootScaleX, boneTransform.initScaleX + o, mixParam);
                }

                if(selfSinScaleXSameAsY || selfScaleXRange == 0)
                {
                    
                }
                o = Mathf.Sin((selfScaleXOffset - Mathf.Pow(selfScaleXChildOffset * index, 1 + selfScaleXSpring) + time) * Mathf.PI * 2 / selfScaleXTime)
        * selfScaleXRange * Mathf.Pow(1 + index * selfScaleXAffectByLevel, 1 + selfSpringLevel) + selfScaleXCenter;
                rootScaleX = Mathf.Lerp(rootScaleX, boneTransform.initScaleX + o, mixParam);

                rootBone.transform.localScale = new Vector3(rootScaleX, rootScaleY, 0);

                foreach (CustomBone child in boneTransform.children)
                {
                    UpdateSineMode(time, child, index + 1, mixParam, endBoneName);
                }

                boneTransform.boneRef.UpdateWorldTransform();
            }
        }

        void UpdateMode3Func(float time,float param)
        {
            float rootBoneX = rootBone.transform.localPosition.x;
            float rootBoneY = rootBone.transform.localPosition.y;
            float rootBoneScaleX = rootBone.transform.localScale.x;
            float rootBoneScaleY = rootBone.transform.localScale.y;
            float rootBoneWorldX = rootBone.transform.position.x;
            float rootBoneWorldY = rootBone.transform.position.y;
            float rootBoneRotation = rootBone.transform.rotation.eulerAngles.z;

            float m = 0 == selfMoveXAmp ? 0 : UpdateWiggleMode(selfMoveXFreq, selfMoveXAmp, selfMoveXOctaves, time, selfMoveXDelay) + selfMoveXCenter;
            rootBoneX = Mathf.Lerp(rootBoneX, rootBone.initX + m, param);
            if (selfMoveYSameAsX)
            {
                m = 0 == selfMoveXAmp ? 0 : UpdateWiggleMode(selfMoveXFreq, selfMoveXAmp, selfMoveXOctaves, time, selfMoveXDelay + selfMoveXSeed) + selfMoveXCenter;
                rootBoneY = Mathf.Lerp(rootBoneY, this.rootBone.initY + m, param);
            }
            else
            {
                m = 0 == selfMoveYCenter ? 0 : UpdateWiggleMode(selfMoveYFreq, selfMoveYAmp, selfMoveYOctaves, time, selfMoveYDelay) + selfMoveYCenter;
		        rootBoneY = Mathf.Lerp(rootBoneY, rootBone.initY + m, param);
            }

            m = 0 == selfScaleXAmp ? 0 : UpdateWiggleMode(selfScaleXFreq, selfScaleXAmp, selfScaleXOctaves, time, selfScaleXDelay) + selfScaleXCenter;
            rootBoneScaleX = Mathf.Lerp(rootBoneScaleX, rootBone.initScaleX + m, param);
            if (selfScaleYSameAsX)
                rootBoneScaleY = Mathf.Lerp(rootBoneScaleY, rootBone.initScaleY + m, param);
            else
            {
                m = 0 == selfScaleYAmp ? 0 : UpdateWiggleMode(selfScaleYFreq, selfScaleYAmp, selfScaleYOctaves, time, selfScaleYDelay) + selfScaleYCenter;
                rootBoneScaleY = Mathf.Lerp(rootBoneScaleY, rootBone.initScaleY + m, param);
            }

            m = rootBone.initRotation + time * selfRotateSpeed * 360 + selfRotateCenter;
            m += 0 == selfRotateAmp ? 0 : UpdateWiggleMode(selfRotateFreq, selfRotateAmp, selfRotateOctaves, time, selfRotateDelay);

            if (selfRotateFollowEnable){

                float Q = rootBoneWorldX - rootBone.autoMovePrevWorldX;
                float W = rootBoneWorldY - rootBone.autoMovePrevWorldY;
                float X = 1 == selfRotateFollowFlip ?
                    -selfRotateFollowLimit * Mathf.Max(-1, Mathf.Min(1, Q / selfRotateFollowXMax)) - selfRotateFollowLimit * Mathf.Max(-1, Mathf.Min(1, W / selfRotateFollowYMax)) :
                    (Mathf.Atan2(W, Q) * selfAffectByLevel + 360) % 360;
	            float G = X - rootBone.followRotation;
                if (G >= 180)
                    X -= 360;
                else if(G <= -180) 
                    X += 360;

                rootBone.followRotation += Mathf.Min(selfRotateFollowLimit, Mathf.Max(-selfRotateFollowLimit, X - rootBone.followRotation)) * selfRotateFollowSpeed;
	            rootBone.followRotation = (rootBone.followRotation + 360) % 360;
                if (2 == selfRotateFollowFlip && Mathf.Abs(rootBone.followRotation - 180) < 90)
                    rootBoneScaleY *= -1;
                m += rootBone.followRotation;
            }
            rootBone.autoMovePrevWorldX = rootBoneWorldX;
            rootBone.autoMovePrevWorldY = rootBoneWorldY;

            rootBoneRotation = Mathf.Lerp(rootBoneRotation, m, param);

            //rootBone.transform.localPosition = new Vector3(rootBoneX, rootBoneY, 0);
            rootBone.boneRef.X = rootBoneX;
            rootBone.boneRef.Y = rootBoneY;
            rootBone.transform.localScale = new Vector3(rootBoneScaleX, rootBoneScaleY, 0);
            rootBone.boneRef.Rotation = rootBoneRotation;

            //rootBone.boneRef.WorldX = rootBoneWorldX;
            //rootBone.boneRef.WorldY = rootBoneWorldY;

            rootBone.boneRef.UpdateWorldTransform();
        }

        float UpdateWiggleMode(float freq, float amp, float octives, float time, float delay, float extra = 0.5f)
        {
            float o = 0,s = 1,u = octives + 1,l = 1 / (2 - 1 / Mathf.Pow(2, u - 1)),c = l,f = 0,d = 0;

            for (;d < u; d++)
            {
                o += s * Mathf.Sin(time * c * Mathf.PI * 2 / freq + delay);
                c = l * Mathf.Pow(2, d + 1);
                f += s;
                s *= extra;
            }
            
            return o / f * amp;
        }

        void UpdateSpringMagic(CustomBone boneTransform, float r, int index, float param, int dimension)
        {
            if (boneTransform.transform.name != selfEndBoneName)
            {
                boneTransform.boneRef.UpdateWorldTransform();
                boneTransform.autoMovePrevWorldX = boneTransform.boneRef.WorldX;
                boneTransform.autoMovePrevWorldY = boneTransform.boneRef.WorldY;
                float l = Mathf.Pow(1 + index * selfAffectByLevel, 1 + selfSpringLevel);
                float c = selfDelay * l * r * (0 == index ? 1 + selfSpring : 1);
                if (boneTransform.children.Count > 0)
                {
                    for (int f = 0; f < boneTransform.children.Count; f++)
                    {
                        var ll = boneTransform.children[f];

                        if (f == 0)
                        {
                            float d = ll.boneRef.X;
                            float p = ll.boneRef.Y;
                            float m = d * boneTransform.boneRef.A + p * boneTransform.boneRef.B + boneTransform.boneRef.WorldX;
                            float h = d * boneTransform.boneRef.C + p * boneTransform.boneRef.D + boneTransform.boneRef.WorldY;

                            m = (m - ll.autoMovePrevWorldX) * c;
                            h = (h - ll.autoMovePrevWorldY) * c;
                            boneTransform.autoMoveSpeedX *= .7f;
                            boneTransform.autoMoveSpeedY *= .7f;
                            float v = ll.autoMovePrevWorldX + boneTransform.autoMoveSpeedX;
                            float A = ll.autoMovePrevWorldY + boneTransform.autoMoveSpeedY;
                            float g = boneTransform.boneRef.WorldToLocalRotation(dimension * Mathf.Atan2(A - boneTransform.boneRef.WorldY, dimension * (v - boneTransform.boneRef.WorldX)) * 180 / Mathf.PI + (0 == index ? selfRotateOffset : 0)),
                            y = Mathf.Min(selfLimitRange, Mathf.Max(-selfLimitRange, g - boneTransform.initRotation)) + boneTransform.initRotation;

                            float targetRotate = (boneTransform.initRotation * selfSpeed + (1 - selfSpeed) * y);
                            boneTransform.boneRef.Rotation = Mathf.Lerp(boneTransform.boneRef.Rotation, boneTransform.initRotation + 0.05f * targetRotate, param * boneTransform.autoMoveFriction);
                            boneTransform.boneRef.UpdateWorldTransform();
                        }
                        UpdateSpringMagic(ll, r, index + 1, param, dimension);
                    }
                }
                else
                {
                    float f = boneTransform.boneRef.X, d = boneTransform.boneRef.Y;
                    float p = f * boneTransform.boneRef.A + d * boneTransform.boneRef.B + boneTransform.boneRef.WorldX;
                    float m = f * boneTransform.boneRef.C + d * boneTransform.boneRef.D + boneTransform.boneRef.WorldY;
                    p = (p - boneTransform.tailAutoMovePrevWorldX) * c;
                    m = (m - boneTransform.tailAutoMovePrevWorldY) * c;
                    boneTransform.autoMoveSpeedX += p;
                    boneTransform.autoMoveSpeedY += m;
                    boneTransform.autoMoveSpeedX *= .7f;
                    boneTransform.autoMoveSpeedY *= .7f;

                    float h = boneTransform.tailAutoMovePrevWorldX + boneTransform.autoMoveSpeedX;
                    float v = boneTransform.tailAutoMovePrevWorldY + boneTransform.autoMoveSpeedY;
                    float A = boneTransform.boneRef.WorldToLocalRotation(dimension * Mathf.Atan2(v - boneTransform.boneRef.WorldY, dimension * (h - boneTransform.boneRef.WorldX)) * 180 / Mathf.PI + (0 == index ? selfRotateOffset : 0));

                    float g = Mathf.Min(selfLimitRange, Mathf.Max(-selfLimitRange, A - boneTransform.initRotation)) + boneTransform.initRotation;
                    float targetRotate = (boneTransform.initRotation * selfSpeed + (1 - selfSpeed) * g);
                    boneTransform.boneRef.Rotation = Mathf.Lerp(boneTransform.boneRef.Rotation, boneTransform.initRotation + 0.05f * targetRotate, param * boneTransform.autoMoveFriction) ;
                    boneTransform.boneRef.UpdateWorldTransform();

                    boneTransform.tailAutoMovePrevWorldX = f * boneTransform.boneRef.A + d * boneTransform.boneRef.B + boneTransform.boneRef.WorldX;
                    boneTransform.tailAutoMovePrevWorldY = f * boneTransform.boneRef.C + d * boneTransform.boneRef.D + boneTransform.boneRef.WorldY;

                }
                boneTransform.autoMoveFriction = .7f * (1 - boneTransform.autoMoveFriction) * r;
                fTime = boneTransform.autoMoveFriction;
            }
        }

    }
}
