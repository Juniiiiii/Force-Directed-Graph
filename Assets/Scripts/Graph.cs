using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class Graph : MonoBehaviour {
    public GameObject nodePrefab;
    private List<Node> nodes = new();
    private List<Spring> springs = new();

    public Vector2 boundary;
    public Vector2 centre;
    public float gravity;
    public int numberOfNodes;
    public float idealLength;

    private Vector2 difference;
    private float distance;
    private Vector2 normalized;

    public float Attraction;
    public float Repulsion;

    private void Start() {
        for (int i = 0 ; i < numberOfNodes; i++) {
            nodes.Add(Instantiate(nodePrefab, new Vector2(UnityEngine.Random.Range(0, boundary.x), UnityEngine.Random.Range(0, boundary.y)), Quaternion.identity).GetComponent<Node>());
        }

        for (int i = 0 ; i < numberOfNodes; i++) {
            for (int j = i + 1; j < numberOfNodes; j++ ) {
                springs.Add(new Spring(nodes[i], nodes[j], idealLength));
            }
        }
    }

    private void Update() {
        ApplyForces();
        DrawSprings();
    }

    private void DrawSprings() {
        foreach(Spring spring in springs) {
            Debug.DrawLine(spring.first.body.position, spring.second.body.position, Color.red);
        }
    }

    private void ApplyForces() {
        //Gravity
        foreach(Node node in nodes) node.netForce = (centre - node.body.position).normalized * Time.deltaTime * gravity/Mathf.Pow(1 + (centre - node.body.position).magnitude, 2);

        //Repulsion
        for(int i = 0 ; i < nodes.Count; i++) {
            for (int j = i + 1; j < nodes.Count; j++) {
                difference = nodes[i].body.position - nodes[j].body.position;
                distance = difference.magnitude;
                normalized = difference.normalized;

                nodes[i].netForce += normalized * Repulsion/(distance * distance) * Time.deltaTime;
                nodes[j].netForce -= normalized * Repulsion/(distance * distance) * Time.deltaTime;
            }
        }

        //Attraction
        foreach(Spring spring in springs) {
            difference = spring.first.body.position - spring.second.body.position;
            distance = difference.magnitude;
            normalized = difference.normalized;

            spring.first.netForce -= normalized * Mathf.Log(distance/spring.length) * Attraction * Time.deltaTime;
            spring.second.netForce += normalized * Mathf.Log(distance/spring.length) * Attraction * Time.deltaTime;
        }

        foreach (Node node in nodes) node.body.AddForce(node.netForce);
    }
}


public class Spring {
    public Node first;
    public Node second;
    public float length;

    public Spring(Node n1, Node n2, float length) {
        first = n1;
        second = n2;
        this.length = length;
    }
}

