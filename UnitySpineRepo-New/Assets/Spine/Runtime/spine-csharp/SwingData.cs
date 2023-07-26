using System;

namespace Spine
{
	public class SwingData
	{
		public string selfAnimationName;
		public string selfStartBoneName;
		public string selfEndBoneName;
		public int selfMode;
        #region mode1
        public float selfRotateCenter;
		public float selfRotateTime;
		public float selfRotateRange;
		public float selfRotateOffset;
		public float selfChildOffset;
		public float selfSpring;
		public float selfAffectByLevel;
		public float selfSpringLevel;
        #endregion
        #region mode3
        public float selfMoveXFreq = 0;
		public float selfMoveXAmp = 0;
		public float selfMoveXOctaves = 0;
		public float selfMoveXDelay = 0;
		public float selfMoveXCenter = 0;
		public float selfMoveXSeed = 0;
		public float selfMoveYFreq = 0;
		public float selfMoveYAmp = 0;
		public float selfMoveYOctaves = 0;
		public float selfMoveYDelay = 0;
		public float selfMoveYCenter = 0;
		public float selfScaleXFreq = 0;
		public float selfScaleXAmp = 0;
		public float selfScaleXOctaves = 0;
		public float selfScaleXDelay = 0;
		public float selfScaleXCenter = 0;
		public float selfScaleYFreq = 0;
		public float selfScaleYAmp = 0;
		public float selfScaleYOctaves = 0;
		public float selfScaleYDelay = 0;
		public float selfScaleYCenter = 0;
		public float selfRotateFollowFlip = 0;
		public float selfRotateFollowLimit = 0;
		public float selfRotateFollowSpeed = 0;
		public float selfRotateFollowXMax = 0;
		public float selfRotateFollowYMax = 0;
		#endregion
		#region mode4
		public float selfDelay = 0;
		public float selfSpeed = 0;
		public float selfLimitRange = 0;
        #endregion

        public SwingData()
		{

		}

		override public string ToString()
		{
			return selfStartBoneName;
		}
	}
}

