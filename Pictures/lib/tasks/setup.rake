desc "Perform initial setup of the Pictures app"
task :setup => ["db:migrate", "sample_data:load"] 
