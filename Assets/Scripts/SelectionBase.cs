using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [SelectionBase] attribute makes it so a GameObject with this script will be "selectable" in the
// editor scene view onclick. Normally, only objects with a renderer are selectable, but the bricks
// all have the renderer on a child, so without this script, clicking will always select the renderer
// child which is very annoying.

[SelectionBase]
public class SelectionBase : MonoBehaviour { }
