using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class comprises static utility and helper methods
/// </summary>
namespace HatsuMuv.DynamixelForUnity
{
    public struct UNIT
    {
        public const double DELAY_TIME = 2;
        public const double POSITION = 0.088;
        public const double VELOCITY = 0.229;
        public const double VOLTAGE = 0.1;
        public const double PWM = 0.113;
        public const double CURRENT = 2.69;
        public const double BUS_WATCHDOG = 20;
        public const double ACCELERATION = 214.577;
    }

    /// <summary>
    /// 0:Current Control Mode  1:Velocity Control Mode  3:Position Control Mode  4:Extended Position Control Mode(Multi-turn)  5:Current-based Position Control Mode  16:PWM Control Mode(Voltage Control Mode)
    /// </summary>
    public enum OperatingMode
    {
        Current = 0,
        Velocity = 1,
        Position = 3,
        ExtendedPosition = 4, //Multi-turn Position Control
        CurrentBasedPosition = 5,
        PWM = 16 //Voltage Control Mode   
    }

    public static class ArrayExtension
    {
        /// <summary>
        /// Divide each value of <paramref name="src"/> by <paramref name="unit"/>.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static uint[] ToRawUInt(this double[] src, double unit)
        {
            UInt32[] dst = new UInt32[src.Length];
            int i = 0;
            foreach (double data in src)
            {
                dst[i] = (uint)(data / unit);
                i = i + 1;
            }
            return dst;
        }

        /// <summary>
        /// Divide each value of <paramref name="src"/> by <paramref name="unit"/>.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static int[] ToRawInt(this double[] src, double unit)
        {
            Int32[] dst = new Int32[src.Length];
            int i = 0;
            foreach (double data in src)
            {
                dst[i] = (int)(data / unit);
                i = i + 1;
            }
            return dst;
        }

        /// <summary>
        /// Multiply each value of <paramref name="src"/> by <paramref name="unit"/>.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double[] ToActualNumber(this uint[] src, double unit)
        {
            double[] dst = new double[src.Length];
            int i = 0;
            foreach (uint data in src)
            {
                dst[i] = (data * unit);
                i = i + 1;
            }
            return dst;
        }
        /// <summary>
        /// Multiply each value of <paramref name="src"/> by <paramref name="unit"/>.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double[] ToActualNumber(this int[] src, double unit)
        {
            double[] dst = new double[src.Length];
            int i = 0;
            foreach (int data in src)
            {
                dst[i] = (data * unit);
                i = i + 1;
            }
            return dst;
        }


        /// <summary>
        /// Converts a uint[] to an int[]
        /// </summary>
        /// <param name="operant"> the uint[] to be converted </param>
        /// <returns></returns>
        public static int[] ToIntArray(this uint[] operant)
        {
            var result = new int[operant.Length];
            for (int i = 0; i < operant.Length; i++)
            {
                result[i] = (int)operant[i];
            }
            return result;
        }
        /// <summary>
        /// convert an int[] to a uint[]
        /// </summary>
        /// <param name="operant">the int[] to be converted</param>
        /// <returns></returns>
        public static uint[] ToUintArray(this int[] operant)
        {
            var result = new uint[operant.Length];
            for (int i = 0; i < operant.Length; i++)
            {
                result[i] = (uint)operant[i];
            }
            return result;
        }
        /// <summary>
        /// converts a uint[] to byte[]
        /// </summary>
        /// <param name="operant"> the uint[] to be converted</param>
        /// <returns></returns>
        public static byte[] ToByteArray(this uint[] operant)
        {
            var result = new byte[operant.Length];
            for (int i = 0; i < operant.Length; i++)
            {
                result[i] = (byte)operant[i];
            }
            return result;
        }

        /// <summary>
        /// convertsa byte[] to uint[] 
        /// </summary>
        /// <param name="operant">byte[] to be converted</param>
        /// <returns></returns>
        public static uint[] ToUintArray(this byte[] operant)
        {
            var result = new uint[operant.Length];
            for (int i = 0; i < operant.Length; i++)
            {
                result[i] = (uint)operant[i];
            }
            return result;
        }

        /// <summary>
        /// converts a uint[] to bool[]
        /// </summary>
        /// <param name="operant">uint[] to be converted</param>
        /// <returns></returns>
        public static bool[] ToBoolArray(this uint[] operant)
        {
            var result = new bool[operant.Length];
            for (int i = 0; i < operant.Length; i++)
            {
                result[i] = operant[i] != 0;
            }
            return result;
        }

        /// <summary>
        /// converts bool[] to uint[]
        /// </summary>
        /// <param name="operant">bool [] to be converted</param>
        /// <returns></returns>
        public static uint[] ToUintArray(this bool[] operant)
        {
            var result = new uint[operant.Length];
            for (int i = 0; i < operant.Length; i++)
            {
                result[i] = operant[i] ? 1u : 0u;
            }
            return result;
        }

        /// <summary>
        /// converts a double[] to float[]
        /// </summary>
        /// <param name="operant">double[] to be converted</param>
        /// <returns></returns>
        public static float[] ToFloatArray(this double[] operant)
        {
            var result = new float[operant.Length];
            for (int i = 0; i < operant.Length; i++)
            {
                result[i] = (float)operant[i];
            }
            return result;
        }
        /// <summary>
        /// this method converts a float[] to a double[]
        /// </summary>
        /// <param name="operant">float[] to be converted</param>
        /// <returns></returns>
        public static double[] ToDoubleArray(this float[] operant)
        {
            var result = new double[operant.Length];
            for (int i = 0; i < operant.Length; i++)
            {
                result[i] = (double)operant[i];
            }
            return result;
        }
    }
}