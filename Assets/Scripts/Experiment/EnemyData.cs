using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Data / Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField, Range(0, 10)] private float moveSpeed;
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material hitMat;
    [SerializeField, Range(0, 1)] private float flashTime;

    public float MoveSpeed => moveSpeed;
    public Material DefaultMat => defaultMat;
    public Material HitMat => hitMat;
    public float FlashTime => flashTime;

}
