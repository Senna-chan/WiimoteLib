using System;
using MathFloat;

namespace WiimoteLib.Helpers{
    class Mahony
    {
        private const float twoKpDef = (2.0f * 0.0f);	// 2 * proportional gain
        private const float twoKiDef = (2.0f * 0.0f); // 2 * integral gain
        float twoKp;        // 2 * proportional gain (Kp)
        float twoKi;        // 2 * integral gain (Ki)
        float q0, q1, q2, q3;   // quaternion of sensor frame relative to auxiliary frame
        float integralFBx, integralFBy, integralFBz;  // integral error terms scaled by Ki
        float invSampleFreq;
        float roll, pitch, yaw;
        bool anglesComputed;
        public Mahony(float refreshRate)
        {
            twoKp = twoKpDef;   // 2 * proportional gain (Kp)
            twoKi = twoKiDef;   // 2 * integral gain (Ki)
            q0 = 1.0f;
            q1 = 0.0f;
            q2 = 0.0f;
            q3 = 0.0f;
            integralFBx = 0.0f;
            integralFBy = 0.0f;
            integralFBz = 0.0f;
            anglesComputed = false;
            invSampleFreq = 1.0f / refreshRate;
        }
        /// <summary>
        /// Updates the quaternion things for the calculations
        /// </summary>
        /// <param name="gx"></param>
        /// <param name="gy"></param>
        /// <param name="gz"></param>
        /// <param name="ax"></param>
        /// <param name="ay"></param>
        /// <param name="az"></param>
        /// <param name="mx"></param>
        /// <param name="my"></param>
        /// <param name="mz"></param>
        public void Update(float gx, float gy, float gz, float ax, float ay, float az, float mx, float my, float mz)
        {
            float recipNorm;
            float q0q0, q0q1, q0q2, q0q3, q1q1, q1q2, q1q3, q2q2, q2q3, q3q3;
            float hx, hy, bx, bz;
            float halfvx, halfvy, halfvz, halfwx, halfwy, halfwz;
            float halfex, halfey, halfez;
            float qa, qb, qc;

            // Use IMU algorithm if magnetometer measurement invalid
            // (avoids NaN in magnetometer normalisation)
            if ((mx == 0.0f) && (my == 0.0f) && (mz == 0.0f))
            {
                UpdateImu(gx, gy, gz, ax, ay, az);
                return;
            }

            // Convert gyroscope degrees/sec to radians/sec
            gx *= 0.0174533f;
            gy *= 0.0174533f;
            gz *= 0.0174533f;

            // Compute feedback only if accelerometer measurement valid
            // (avoids NaN in accelerometer normalisation)
            if (!((ax == 0.0f) && (ay == 0.0f) && (az == 0.0f)))
            {

                // Normalise accelerometer measurement
                recipNorm = InvSqrt(ax * ax + ay * ay + az * az);
                ax *= recipNorm;
                ay *= recipNorm;
                az *= recipNorm;

                // Normalise magnetometer measurement
                recipNorm = InvSqrt(mx * mx + my * my + mz * mz);
                mx *= recipNorm;
                my *= recipNorm;
                mz *= recipNorm;

                // Auxiliary variables to avoid repeated arithmetic
                q0q0 = q0 * q0;
                q0q1 = q0 * q1;
                q0q2 = q0 * q2;
                q0q3 = q0 * q3;
                q1q1 = q1 * q1;
                q1q2 = q1 * q2;
                q1q3 = q1 * q3;
                q2q2 = q2 * q2;
                q2q3 = q2 * q3;
                q3q3 = q3 * q3;

                // Reference direction of Earth's magnetic field
                hx = 2.0f * (mx * (0.5f - q2q2 - q3q3) + my * (q1q2 - q0q3) + mz * (q1q3 + q0q2));
                hy = 2.0f * (mx * (q1q2 + q0q3) + my * (0.5f - q1q1 - q3q3) + mz * (q2q3 - q0q1));
                bx = MathF.Sqrt(hx * hx + hy * hy);
                bz = 2.0f * (mx * (q1q3 - q0q2) + my * (q2q3 + q0q1) + mz * (0.5f - q1q1 - q2q2));

                // Estimated direction of gravity and magnetic field
                halfvx = q1q3 - q0q2;
                halfvy = q0q1 + q2q3;
                halfvz = q0q0 - 0.5f + q3q3;
                halfwx = bx * (0.5f - q2q2 - q3q3) + bz * (q1q3 - q0q2);
                halfwy = bx * (q1q2 - q0q3) + bz * (q0q1 + q2q3);
                halfwz = bx * (q0q2 + q1q3) + bz * (0.5f - q1q1 - q2q2);

                // Error is sum of cross product between estimated direction
                // and measured direction of field vectors
                halfex = (ay * halfvz - az * halfvy) + (my * halfwz - mz * halfwy);
                halfey = (az * halfvx - ax * halfvz) + (mz * halfwx - mx * halfwz);
                halfez = (ax * halfvy - ay * halfvx) + (mx * halfwy - my * halfwx);

                // Compute and apply integral feedback if enabled
                if (twoKi > 0.0f)
                {
                    // integral error scaled by Ki
                    integralFBx += twoKi * halfex * invSampleFreq;
                    integralFBy += twoKi * halfey * invSampleFreq;
                    integralFBz += twoKi * halfez * invSampleFreq;
                    gx += integralFBx;  // apply integral feedback
                    gy += integralFBy;
                    gz += integralFBz;
                }
                else
                {
                    integralFBx = 0.0f; // prevent integral windup
                    integralFBy = 0.0f;
                    integralFBz = 0.0f;
                }

                // Apply proportional feedback
                gx += twoKp * halfex;
                gy += twoKp * halfey;
                gz += twoKp * halfez;
            }

            // Integrate rate of change of quaternion
            gx *= (0.5f * invSampleFreq);       // pre-multiply common factors
            gy *= (0.5f * invSampleFreq);
            gz *= (0.5f * invSampleFreq);
            qa = q0;
            qb = q1;
            qc = q2;
            q0 += (-qb * gx - qc * gy - q3 * gz);
            q1 += (qa * gx + qc * gz - q3 * gy);
            q2 += (qa * gy - qb * gz + q3 * gx);
            q3 += (qa * gz + qb * gy - qc * gx);

            // Normalise quaternion
            recipNorm = InvSqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
            q0 *= recipNorm;
            q1 *= recipNorm;
            q2 *= recipNorm;
            q3 *= recipNorm;
            anglesComputed = false;
        }

        //-------------------------------------------------------------------------------------------
        // IMU algorithm update
        /// <summary>
        /// quaternion
        /// </summary>
        /// <param name="gx">Gyro X in deg/sec</param>
        /// <param name="gy">Gyro Y in deg/sec</param>
        /// <param name="gz">Gyro Z in deg/sec</param>
        /// <param name="ax">Accel X in G</param>
        /// <param name="ay">Accel Y in G</param>
        /// <param name="az">Accel Z in G</param>
        public void UpdateImu(float gx, float gy, float gz, float ax, float ay, float az, float refreshTiming = 0)
        {
            if (refreshTiming != 0)
            {
                invSampleFreq = 1.0f / refreshTiming;
            }
            float recipNorm;
            float halfvx, halfvy, halfvz;
            float halfex, halfey, halfez;
            float qa, qb, qc;

            // Convert gyroscope degrees/sec to radians/sec
            gx *= 0.0174533f;
            gy *= 0.0174533f;
            gz *= 0.0174533f;

            // Compute feedback only if accelerometer measurement valid
            // (avoids NaN in accelerometer normalisation)
            if (!((ax == 0.0f) && (ay == 0.0f) && (az == 0.0f)))
            {

                // Normalise accelerometer measurement
                recipNorm = InvSqrt(ax * ax + ay * ay + az * az);
                ax *= recipNorm;
                ay *= recipNorm;
                az *= recipNorm;

                // Estimated direction of gravity
                halfvx = q1 * q3 - q0 * q2;
                halfvy = q0 * q1 + q2 * q3;
                halfvz = q0 * q0 - 0.5f + q3 * q3;

                // Error is sum of cross product between estimated
                // and measured direction of gravity
                halfex = (ay * halfvz - az * halfvy);
                halfey = (az * halfvx - ax * halfvz);
                halfez = (ax * halfvy - ay * halfvx);

                // Compute and apply integral feedback if enabled
                if (twoKi > 0.0f)
                {
                    // integral error scaled by Ki
                    integralFBx += twoKi * halfex * invSampleFreq;
                    integralFBy += twoKi * halfey * invSampleFreq;
                    integralFBz += twoKi * halfez * invSampleFreq;
                    gx += integralFBx;  // apply integral feedback
                    gy += integralFBy;
                    gz += integralFBz;
                }
                else
                {
                    integralFBx = 0.0f; // prevent integral windup
                    integralFBy = 0.0f;
                    integralFBz = 0.0f;
                }

                // Apply proportional feedback
                gx += twoKp * halfex;
                gy += twoKp * halfey;
                gz += twoKp * halfez;
            }

            // Integrate rate of change of quaternion
            gx *= (0.5f * invSampleFreq);       // pre-multiply common factors
            gy *= (0.5f * invSampleFreq);
            gz *= (0.5f * invSampleFreq);
            qa = q0;
            qb = q1;
            qc = q2;
            q0 += (-qb * gx - qc * gy - q3 * gz);
            q1 += (qa * gx + qc * gz - q3 * gy);
            q2 += (qa * gy - qb * gz + q3 * gx);
            q3 += (qa * gz + qb * gy - qc * gx);

            // Normalise quaternion
            recipNorm = InvSqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
            q0 *= recipNorm;
            q1 *= recipNorm;
            q2 *= recipNorm;
            q3 *= recipNorm;
            anglesComputed = false;
        }

        private float InvSqrt(float x)
        {
            float xhalf = 0.5f * x;
            int i = BitConverter.ToInt32(BitConverter.GetBytes(x), 0);
            i = 0x5f3759df - (i >> 1);
            x = BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
            x = x * (1.5f - xhalf * x * x);
            return x;
        }

        public void ComputeAngles()
        {
            roll = MathF.Atan2(q0 * q1 + q2 * q3, 0.5f - q1 * q1 - q2 * q2);
            pitch = MathF.Asin(-2.0f * (q1 * q3 - q0 * q2));
            yaw = MathF.Atan2(q1 * q2 + q0 * q3, 0.5f - q2 * q2 - q3 * q3);
            anglesComputed = true;
        }

        public float GetRoll()
        {
            if (!anglesComputed) ComputeAngles();
            return roll * 57.29578f;
        }
        public float GetPitch()
        {
            if (!anglesComputed) ComputeAngles();
            return pitch * 57.29578f;
        }
        public float GetYaw()
        {
            if (!anglesComputed) ComputeAngles();
            return yaw * 57.29578f + 180.0f;
        }
        public float GetRollRadians()
        {
            if (!anglesComputed) ComputeAngles();
            return roll;
        }
        public float GetPitchRadians()
        {
            if (!anglesComputed) ComputeAngles();
            return pitch;
        }
        public float GetYawRadians()
        {
            if (!anglesComputed) ComputeAngles();
            return yaw;
        }
    }
}