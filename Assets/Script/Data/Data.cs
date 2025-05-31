using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public interface Data
{
    public Task Initialize(FirebaseDatabase reference);
}
