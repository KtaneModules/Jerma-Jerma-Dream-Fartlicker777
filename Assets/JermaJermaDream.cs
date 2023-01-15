using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class JermaJermaDream : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;
   public KMNeedyModule Needy;

   public Material[] photos;
   public GameObject screen;
   public KMSelectable[] buttons;


   int selectedphoto = -1;


   //Logging
   static int moduleIdCounter = 1;
   int moduleId;
   private bool moduleSolved;

   void Awake () {
      moduleId = moduleIdCounter++;
      Needy = GetComponent<KMNeedyModule>();
      Needy.OnNeedyActivation += OnNeedyActivation;
      Needy.OnNeedyDeactivation += OnNeedyDeactivation;
      Needy.OnTimerExpired += OnTimerExpired;


      foreach (KMSelectable button in buttons) {
         button.OnInteract += delegate () { buttonPress(button); return false; };
      }


      //button.OnInteract += delegate () { PressButton(); return false; };

   }

   // Use this for initialization
   void Start () {
      moduleSolved = true;

   }

   void OnNeedyActivation () {
      moduleSolved = false;
      selectedphoto = UnityEngine.Random.Range(0, 3);
      screen.GetComponent<MeshRenderer>().material = photos[selectedphoto];
      if (selectedphoto == 0) {
         Audio.PlaySoundAtTransform("Jerma", transform);
      }
   }

   void OnNeedyDeactivation () {
      moduleSolved = true;
   }

   void OnTimerExpired () {
      Needy.HandleStrike();
      moduleSolved = true;
   }

   void buttonPress (KMSelectable button) {
      if (moduleSolved == false) {
         if ((button == buttons[0]) && (selectedphoto == 0)) {
            Needy.HandlePass();
         }
         else if ((button == buttons[1]) && (selectedphoto == 1)) {
            Needy.HandlePass();
         }
         else if ((button == buttons[2]) && (selectedphoto == 2)) {
            Needy.HandlePass();
         }
         else {
            Needy.HandleStrike();
            Needy.HandlePass();
         }
         moduleSolved = true;
      }
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} Jerma/Innocent/Dream to press that corresponding button.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      Command = Command.Trim().ToUpper();
      yield return null;
      if (Command == "JERMA")
         buttons[0].OnInteract();
      else if (Command == "INNOCENT")
         buttons[1].OnInteract();
      else if (Command == "DREAM")
         buttons[2].OnInteract();
      else
         yield return "sendtochaterror Invalid command!";
   }

   void TwitchHandleForcedSolve()
   { 
      StartCoroutine(HandleAutosolve());
   }

   IEnumerator HandleAutosolve()
   {
      while (true)
      {
         while (moduleSolved) yield return null;
         buttons[selectedphoto].OnInteract();
      }
   }//coding in github is a fucking nightmare with tabbing
}
