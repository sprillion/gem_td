#!/bin/bash
# Script to create UI prefabs using MCP

MCP="http://localhost:56342"
ID=100

call_mcp() {
    local method=$1
    local name=$2
    local args=$3
    ID=$((ID+1))
    echo "{\"jsonrpc\":\"2.0\",\"id\":$ID,\"method\":\"tools/call\",\"params\":{\"name\":\"$name\",\"arguments\":$args}}" > /tmp/mcp_req.json
    curl -s -X POST $MCP -H "Content-Type: application/json" --data @/tmp/mcp_req.json
    sleep 0.5
}

echo "Creating AbilityItem components..."

# Add HorizontalLayoutGroup
call_mcp "add" "gameobject-component-add" '{"gameObjectRef":{"path":"AbilityItem"},"componentType":"HorizontalLayoutGroup"}'

# Add LayoutElement
call_mcp "add" "gameobject-component-add" '{"gameObjectRef":{"path":"AbilityItem"},"componentType":"LayoutElement"}'

# Modify LayoutElement
call_mcp "modify" "gameobject-component-modify" '{"gameObjectRef":{"path":"AbilityItem"},"componentType":"LayoutElement","properties":{"preferredHeight":60}}'

# Modify HorizontalLayoutGroup
call_mcp "modify" "gameobject-component-modify" '{"gameObjectRef":{"path":"AbilityItem"},"componentType":"HorizontalLayoutGroup","properties":{"spacing":10,"childControlWidth":true,"childControlHeight":true,"childForceExpandWidth":false,"childForceExpandHeight":false}}'

# Create Icon child
call_mcp "create" "gameobject-create" '{"name":"Icon","parentPath":"AbilityItem","componentTypes":["RectTransform","Image"]}'

# Create Info child
call_mcp "create" "gameobject-create" '{"name":"Info","parentPath":"AbilityItem","componentTypes":["RectTransform","VerticalLayoutGroup"]}'

# Create Name text
call_mcp "create" "gameobject-create" '{"name":"Name","parentPath":"AbilityItem/Info","componentTypes":["RectTransform"]}'

# Add TextMeshProUGUI to Name
call_mcp "add" "gameobject-component-add" '{"gameObjectRef":{"path":"AbilityItem/Info/Name"},"componentType":"TMPro.TextMeshProUGUI"}'

# Create Description text
call_mcp "create" "gameobject-create" '{"name":"Description","parentPath":"AbilityItem/Info","componentTypes":["RectTransform"]}'

# Add TextMeshProUGUI to Description
call_mcp "add" "gameobject-component-add" '{"gameObjectRef":{"path":"AbilityItem/Info/Description"},"componentType":"TMPro.TextMeshProUGUI"}'

# Add AbilityItemUI script
call_mcp "add" "gameobject-component-add" '{"gameObjectRef":{"path":"AbilityItem"},"componentType":"ui.selection.AbilityItemUI"}'

# Create prefab folder if needed
call_mcp "create" "assets-create-folder" '{"folderPath":"Assets/Resources/Prefabs/UI"}'

# Save as prefab
call_mcp "prefab" "assets-prefab-create" '{"gameObjectRef":{"path":"AbilityItem"},"prefabPath":"Assets/Resources/Prefabs/UI/AbilityItem.prefab"}'

echo "AbilityItem prefab created!"
