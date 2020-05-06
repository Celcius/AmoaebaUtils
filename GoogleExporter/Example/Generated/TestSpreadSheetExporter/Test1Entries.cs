using UnityEngine;
using System;
using System.Collections;


// !!!!! WARNING !!!!!
// Auto Generated Class --- DO NOT CHANGE ---


namespace Config {
public class Test1Entries : ScriptableObject {
 

 public Test1Entry[] entries;

 private void Populate(Array array) {
  entries= new Test1Entry[array.Length];
  for(int i = 0; i < array.Length;i++)
  {
   entries[i] = (Test1Entry)array.GetValue(i);
  }
 }

 }
}
