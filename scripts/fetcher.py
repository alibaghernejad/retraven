import json
import os

# Define the path to the contents JSON file
contents_file = "gapfilm-contents.json"

# Directory where the individual JSON files will be saved
output_dir = "./individual_json_files"

# Make sure the output directory exists
if not os.path.exists(output_dir):
    os.makedirs(output_dir)

# Read the contents JSON file
with open(contents_file, 'r') as file:
    doc = json.load(file)

# Function to create a JSON file for each individual hit inside contents
def create_json_files(content_data):
    hits = content_data.get("Hits", [])
    for hit in hits:
        # Define the output file path for each individual
        file_path = os.path.join(output_dir, f"{hit['Id']}.json")

        # Serialize the hit data to a JSON string
        json_string = json.dumps(hit, indent=4)

        # Write the serialized JSON string to the file
        with open(file_path, 'w') as json_file:
            json_file.write(json_string)

        print(f"Created JSON file for individual {hit['Id']} at {file_path}")

# Call the function to create JSON files for each individual hit
create_json_files(doc)
