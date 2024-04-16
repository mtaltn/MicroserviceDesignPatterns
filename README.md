# MicroserviceDesignPatterns

# Saga Pattern Nedir?
Saga Pattern ile oluşturulan sistemlerde gelen istek ile daha sonraki her adım, bir önceki adımın başarılı şekilde tamamlanması sonrasında tetiklenir. Herhangi bir failover durumunda işlemi geri alma veya bir düzeltme aksiyonu almayı sağlayan pattern’dir.

Orchestration-Based Saga
Bu yaklaşımda tüm işlemleri yöneten bir Saga Orchestrator’u vardır. Bu orchestrator subscribe olan tüm consumer’lara ne zaman ne yapacağını ileten bir consumer’dır.

Choreography-Based Saga
Bu yaklaşımda merkezi bir yönetici yoktur. Her servis işlemi tamamlar ve event fırlatır ve bir sonraki servis ilgili event’i consume edip sürecine devam eder.

# Event Sourcing Pattern Nedir ?
Event Sourcing, bir sistemin durumunu olaylar aracılığıyla takip ettiği bir tasarım desenidir. Bu yaklaşımda, tüm sistem durumu, geçmişte gerçekleşen olayların bir koleksiyonu olarak saklanır ve her bir olay sistemin geçerli durumunu oluşturan tekil bir değişikliği temsil eder.
