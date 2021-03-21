<template>
  <Portal to="app-popup-detail">
    <PopupBaseInputDetail
      :label="label"
      @close="$router.back()"
    >
      <slot name="backdrop" slot="backdrop"/>
      <slot/>
    </PopupBaseInputDetail>
  </Portal>
</template>

<script>
import PopupBaseInputDetail from './PopupBaseInputDetail.vue';

export default {
  name: 'PopupRouterViewInputDetail',
  props: {
    label: {
      required: true,
      type: String,
    },
  },
  components: {
    PopupBaseInputDetail,
  },
};
</script>