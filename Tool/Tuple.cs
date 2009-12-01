// Tuple.cs
// ------------------------------------------------------------------
//
// Description goes here....
// 
// Author: Admin
// built on host: DINOCH-2
// Created Tue Dec 01 17:30:44 2009
//
// last saved: 
// Time-stamp: <2009-December-01 17:32:08>
// ------------------------------------------------------------------
//
// Copyright (c) 2009 by Dino Chiesa
// All rights reserved!
//
// ------------------------------------------------------------------

namespace XPathVisualizer
{
    public static class Tuple
    {
        //Allows Tuple.New(1, "2") instead of new Tuple<int, string>(1, "2")
        public static Tuple<T1, T2> New<T1, T2>(T1 v1, T2 v2)
        {
            return new Tuple<T1, T2>(v1, v2);
        }
    }
    
    public class Tuple<T1, T2>
    {
        public Tuple(T1 v1, T2 v2)
        {
            V1 = v1;
            V2 = v2;
        }
    
        public T1 V1 { get; set; }
        public T2 V2 { get; set; }
    }
}
