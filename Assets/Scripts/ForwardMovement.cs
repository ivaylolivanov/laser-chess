using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardMovement : Movement {
    [SerializeField] private Vector3Int direction;

    private void Awake() {
        base.Awake();

        direction.x = Mathf.Clamp(direction.x, -1, 1);
        direction.y = Mathf.Clamp(direction.y, -1, 1);
    }

    public override void GetAvailablePositions() {
        boardGrid.GetForwardPositions(
            transform.position,
            out allDirections,
            direction,
            stats.stepsLimit,
            Utils.TILE_VALUE_FREE
        );
    }

    public virtual IEnumerator Move(Vector2 position) {
        Vector2 originalPosition = transform.position;

        transform.position = allDirections[0];

        yield return new WaitForSeconds(stats.intervalBetweenSteps);

        boardGrid.UpdateTile(originalPosition);

        hasMoved = true;
    }

    protected override void FillMeshVertices(ref Vector3[] meshVertices) {
        if(allDirections.Count <= 0) { return; }

        int index = 0;
        Vector2 bottomLeft = allDirections[0];
        Vector2 topRight = allDirections[allDirections.Count - 1] + Vector2.one;

        if(direction.x < 0 || direction.y < 0) {
            bottomLeft = allDirections[allDirections.Count - 1];
            topRight = allDirections[0] + Vector2.one;
        }

        Vector3[] rectVertices = Utils.GetRectangleVertices(
            bottomLeft,
            topRight
        );

        foreach(Vector3 vertex in rectVertices) {
            meshVertices[index] = vertex - transform.position;
            ++index;
        }
    }

    protected override int CalculateNumberOfRectangles() {
        int vertices = (allDirections.Count > 0) ? 1 : 0;

        return vertices;
    }
}
