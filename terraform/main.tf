provider "google" {
  project = var.project_id
  zone    = var.zone
}

resource "google_container_cluster" "primary" {
  name     = "swiftnews-1"
  location = var.zone

  remove_default_node_pool = true

  initial_node_count = 3
}

resource "google_container_node_pool" "primary_preemptible_nodes" {
  cluster    = google_container_cluster.primary.name
  location   = var.zone
  name       = "preemptible-node-pool"

  node_config {
    preemptible  = true
    machine_type = "e2-medium"
    disk_size_gb = 20

    oauth_scopes = [
      "https://www.googleapis.com/auth/devstorage.read_only",
      "https://www.googleapis.com/auth/logging.write",
      "https://www.googleapis.com/auth/monitoring",
      "https://www.googleapis.com/auth/servicecontrol",
      "https://www.googleapis.com/auth/service.management.readonly",
      "https://www.googleapis.com/auth/trace.append",
    ]
  }

  initial_node_count = 3

  management {
    auto_upgrade = true
    auto_repair  = true
  }
}