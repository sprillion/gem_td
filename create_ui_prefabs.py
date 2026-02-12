#!/usr/bin/env python3
"""
Script to create Selection Info Panel UI prefabs using Unity MCP server
"""
import json
import urllib.request
import re

MCP_URL = "http://localhost:56342"

def call_mcp_tool(tool_name, arguments):
    """Call MCP tool and return result"""
    payload = {
        "jsonrpc": "2.0",
        "id": 1,
        "method": "tools/call",
        "params": {
            "name": tool_name,
            "arguments": arguments
        }
    }

    data = json.dumps(payload).encode('utf-8')
    req = urllib.request.Request(MCP_URL, data=data, headers={'Content-Type': 'application/json'})

    try:
        with urllib.request.urlopen(req, timeout=30) as response:
            content = response.read().decode('utf-8')
            # Parse SSE format (event: message\ndata: {...})
            match = re.search(r'data: ({.*})', content, re.DOTALL)
            if match:
                result = json.loads(match.group(1))
                if 'result' in result:
                    return result['result']
                elif 'error' in result:
                    print(f"Error: {result['error']}")
                    return None
            return None
    except Exception as e:
        print(f"Error calling {tool_name}: {e}")
        return None

def create_gameobject(name, parent_path=None, components=None):
    """Create a GameObject"""
    args = {"name": name}
    if parent_path:
        args["parentPath"] = parent_path
    if components:
        args["componentTypes"] = components

    result = call_mcp_tool("gameobject-create", args)
    if result:
        print(f"Created GameObject: {name}")
        return result
    return None

def add_component(gameobject_path, component_type):
    """Add component to GameObject"""
    result = call_mcp_tool("gameobject-component-add", {
        "gameObjectRef": {"path": gameobject_path},
        "componentType": component_type
    })
    if result:
        print(f"Added {component_type} to {gameobject_path}")
    return result

def modify_component(gameobject_path, component_type, properties):
    """Modify component properties"""
    result = call_mcp_tool("gameobject-component-modify", {
        "gameObjectRef": {"path": gameobject_path},
        "componentType": component_type,
        "properties": properties
    })
    if result:
        print(f"Modified {component_type} on {gameobject_path}")
    return result

def create_prefab(gameobject_path, prefab_path):
    """Create prefab from GameObject"""
    result = call_mcp_tool("assets-prefab-create", {
        "gameObjectRef": {"path": gameobject_path},
        "prefabPath": prefab_path
    })
    if result:
        print(f"Created prefab: {prefab_path}")
    return result

def destroy_gameobject(gameobject_path):
    """Destroy GameObject"""
    result = call_mcp_tool("gameobject-destroy", {
        "gameObjectRef": {"path": gameobject_path}
    })
    if result:
        print(f"Destroyed: {gameobject_path}")
    return result

# Main execution
print("Creating Selection Info Panel UI Prefabs...")
print("=" * 60)

# 1. Create AbilityItem prefab
print("\n1. Creating AbilityItem prefab...")
create_gameobject("AbilityItem", components=["RectTransform", "HorizontalLayoutGroup"])
modify_component("AbilityItem", "HorizontalLayoutGroup", {
    "spacing": 10,
    "childForceExpandWidth": True,
    "childForceExpandHeight": False,
    "childControlWidth": True,
    "childControlHeight": True
})
modify_component("AbilityItem", "RectTransform", {
    "sizeDelta": {"x": 350, "y": 60}
})

# Icon
create_gameobject("Icon", parent_path="AbilityItem", components=["RectTransform", "Image"])
modify_component("AbilityItem/Icon", "RectTransform", {
    "sizeDelta": {"x": 40, "y": 40}
})
modify_component("AbilityItem/Icon", "LayoutElement", {
    "preferredWidth": 40,
    "preferredHeight": 40,
    "flexibleWidth": 0,
    "flexibleHeight": 0
})
add_component("AbilityItem/Icon", "LayoutElement")

# Info container
create_gameobject("Info", parent_path="AbilityItem", components=["RectTransform", "VerticalLayoutGroup"])
modify_component("AbilityItem/Info", "VerticalLayoutGroup", {
    "spacing": 2,
    "childForceExpandWidth": True,
    "childForceExpandHeight": False
})

# Name text
create_gameobject("Name", parent_path="AbilityItem/Info", components=["RectTransform", "TextMeshProUGUI"])
modify_component("AbilityItem/Info/Name", "TextMeshProUGUI", {
    "text": "Ability Name",
    "fontSize": 14,
    "fontStyle": 1,  # Bold
    "color": {"r": 1, "g": 1, "b": 1, "a": 1}
})

# Description text
create_gameobject("Description", parent_path="AbilityItem/Info", components=["RectTransform", "TextMeshProUGUI"])
modify_component("AbilityItem/Info/Description", "TextMeshProUGUI", {
    "text": "Ability description",
    "fontSize": 11,
    "color": {"r": 0.8, "g": 0.8, "b": 0.8, "a": 1}
})

# Add AbilityItemUI component
add_component("AbilityItem", "ui.selection.AbilityItemUI")

# Create prefab
create_prefab("AbilityItem", "Assets/Resources/Prefabs/UI/AbilityItem.prefab")
destroy_gameobject("AbilityItem")

print("\n" + "=" * 60)
print("All UI prefabs created successfully!")
print("Prefabs location: Assets/Resources/Prefabs/UI/")
